using Unity.Burst;
using Unity.Mathematics;

namespace Unity.Audio
{
    [BurstCompile]
    public struct MixNode : IAudioKernel<MixNode.Parameters, MixNode.Providers>
    {
        public enum Parameters
        {
            [ParameterDefault(0.5f)] [ParameterRange(0f, 1f)]
            Mix
        }

        public enum Providers
        {
        }

        public void Initialize()
        {
        }

        public void Execute(ref ExecuteContext<Parameters, Providers> context)
        {
            if (context.Inputs.Count != 2 || context.Outputs.Count != 1)
            {
                return;
            }

            var inputBufferOne = context.Inputs.GetSampleBuffer(0).Buffer;
            var inputChannelsOne = context.Inputs.GetSampleBuffer(0).Channels;
            var inputBufferTwo = context.Inputs.GetSampleBuffer(1).Buffer;
            var inputChannelsTwo = context.Inputs.GetSampleBuffer(1).Channels;
            
            var outputBuffer = context.Outputs.GetSampleBuffer(0).Buffer;
            var outputChannels = context.Outputs.GetSampleBuffer(0).Channels;

            var frames = outputBuffer.Length / outputChannels;
            var parameters = context.Parameters;
            for (int s = 0, i = 0; s < frames; s++)
            {
                float m = parameters.GetFloat(Parameters.Mix, s);
                for (var c = 0; c < outputChannels; c++)
                {
                    // what if there are different number of channels???
                    outputBuffer[i] = inputBufferOne[i]*(1f-m) + inputBufferTwo[i]*m;
                    i++;
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
