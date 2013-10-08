using System;
using JFlacLib.JFlac.IO;

namespace JFlacLib.JFlac.Frame
{
    public class EntropyPartitionedRice : EntropyCodingMethod
    {
        private const int EntropyCodingMethodPartitionedRiceParameterLen = 4; /* bits */
        private const int EntropyCodingMethodPartitionedRiceRawLen = 5; /* bits */
        private const int EntropyCodingMethodPartitionedRiceEscapeParameter = 15;

        internal int Order; // The partition order, i.e. # of contexts = 2 ^ order.
        internal EntropyPartitionedRiceContents Contents; // The context's Rice parameters and/or raw bits.

        internal void ReadResidual(BitInputStream inputStream, int predictorOrder, int partitionOrder, Header header,
                                  ref int[] residual)
        {
            //System.out.println("readREsidual Pred="+predictorOrder+" part="+partitionOrder);
            int sample = 0;
            int partitions = 1 << partitionOrder;
            int partitionSamples = partitionOrder > 0
                                       ? header.blockSize >> partitionOrder
                                       : header.blockSize - predictorOrder;
            Contents.EnsureSize(Math.Max(6, partitionOrder));
            Contents.Parameters = new int[partitions];

            for (int partition = 0; partition < partitions; partition++)
            {
                int riceParameter = inputStream.ReadRawUInt(EntropyCodingMethodPartitionedRiceParameterLen);
                Contents.Parameters[partition] = riceParameter;
                if (riceParameter < EntropyCodingMethodPartitionedRiceEscapeParameter)
                {
                    int u = (partitionOrder == 0 || partition > 0)
                                ? partitionSamples
                                : partitionSamples - predictorOrder;
                    inputStream.ReadRiceSignedBlock(ref residual, sample, u, riceParameter);
                    sample += u;
                }
                else
                {
                    riceParameter = inputStream.ReadRawUInt(EntropyCodingMethodPartitionedRiceRawLen);
                    Contents.RawBits[partition] = riceParameter;
                    for (int u = (partitionOrder == 0 || partition > 0) ? 0 : predictorOrder;
                         u < partitionSamples;
                         u++, sample++)
                    {
                        residual[sample] = inputStream.ReadRawInt(riceParameter);
                    }
                }
            }
        }
    }
}
