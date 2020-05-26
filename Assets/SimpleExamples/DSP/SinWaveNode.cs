using Unity.Burst;
using Unity.Mathematics;

namespace Unity.Audio
{
    [BurstCompile]
    public struct SinWaveNode : IAudioKernel<SinWaveNode.Parameters, SinWaveNode.Providers>
    {
        public enum Parameters
        {
            [ParameterDefault(300.0f)] [ParameterRange(200.0f, 1800.0f)]
            Frequency
        }

        public enum Providers
        {
        }

        float m_Phase;

        public void Initialize()
        {
        }

        public void Execute(ref ExecuteContext<Parameters, Providers> context)
        {
            if (context.Outputs.Count == 0)
                return;

            var outputBuffer = context.Outputs.GetSampleBuffer(0).Buffer;
            var outputChannels = context.Outputs.GetSampleBuffer(0).Channels;

            var frames = outputBuffer.Length / outputChannels;
            var parameters = context.Parameters;
            for (int s = 0, i = 0; s < frames; s++)
            {
                
                float output = math.sin(m_Phase * math.PI);
                for (var c = 0; c < outputChannels; c++)
                {
                    outputBuffer[i++] = output;
                }
                
                float delta = parameters.GetFloat(Parameters.Frequency, s) / context.SampleRate;
                m_Phase += delta;
                m_Phase -= math.floor(m_Phase);
            }
        }

        public void Dispose()
        {
        }
    }
}
