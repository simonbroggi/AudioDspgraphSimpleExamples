using System;
using Unity.Audio;
using UnityEngine;

public class ScheduleParametersWave : MonoBehaviour
{
    public float Frequency = 300.0f;
    public float Mix = 0.5f;
    DSPGraph m_Graph;
    DSPNode m_SineWave;
    DSPNode m_SawWave;
    DSPNode m_MixNode;

    void Start()
    {
        var format = ChannelEnumConverter.GetSoundFormatFromSpeakerMode(AudioSettings.speakerMode);
        var channels = ChannelEnumConverter.GetChannelCountFromSoundFormat(format);
        AudioSettings.GetDSPBufferSize(out var bufferLength, out var numBuffers);
        var sampleRate = AudioSettings.outputSampleRate;

        m_Graph = DSPGraph.Create(format, channels, bufferLength, sampleRate);

        var driver = new DefaultDSPGraphDriver { Graph = m_Graph };
        driver.AttachToDefaultOutput();

        using (var block = m_Graph.CreateCommandBlock())
        {
            m_SineWave = block.CreateDSPNode<SinWaveNode.Parameters, SinWaveNode.Providers, SinWaveNode>();
            block.AddOutletPort(m_SineWave, 2, SoundFormat.Stereo);
            m_SawWave = block.CreateDSPNode<SawWaveNode.Parameters, SawWaveNode.Providers, SawWaveNode>();
            block.AddOutletPort(m_SawWave, 2, SoundFormat.Stereo);

            m_MixNode = block.CreateDSPNode<MixNode.Parameters, MixNode.Providers, MixNode>();
            block.AddInletPort(m_MixNode, 2, SoundFormat.Stereo);
            block.AddInletPort(m_MixNode, 2, SoundFormat.Stereo);
            block.AddOutletPort(m_MixNode, 2, SoundFormat.Stereo);

            block.Connect(m_SineWave, 0, m_MixNode, 0);
            block.Connect(m_SawWave, 0, m_MixNode, 1);
            block.Connect(m_MixNode, 0, m_Graph.RootDSP, 0);
        }
    }

    void Update()
    {
        m_Graph.Update();
    }

    void OnDestroy()
    {
        using (var block = m_Graph.CreateCommandBlock())
        {
            block.ReleaseDSPNode(m_SineWave);
            block.ReleaseDSPNode(m_SawWave);
            block.ReleaseDSPNode(m_MixNode);
        }
    }

    void OnGUI()
    {
        using (var block = m_Graph.CreateCommandBlock())
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(20, 70, 300, 30), "Frequency: " + Frequency);
            var newFrequency = GUI.HorizontalSlider(new Rect(20, 100, 600, 30), Frequency, 20.0f, 20000.0f);
            if (Math.Abs(newFrequency - Frequency) > 0.0001f)
            {
                block.SetFloat<SinWaveNode.Parameters, SinWaveNode.Providers, SinWaveNode>(m_SineWave, SinWaveNode.Parameters.Frequency, newFrequency);
                block.SetFloat<SawWaveNode.Parameters, SawWaveNode.Providers, SawWaveNode>(m_SawWave, SawWaveNode.Parameters.Frequency, newFrequency);
                Frequency = newFrequency;
            }

            GUI.Label(new Rect(20, 170, 300, 30), "Mix: " + Mix);
            var newMix = GUI.HorizontalSlider(new Rect(20, 200, 600, 30), Mix, 0f, 1f);
            if (Math.Abs(newMix - Mix) > 0.0001f)
            {
                block.SetFloat<MixNode.Parameters, MixNode.Providers, MixNode>(m_MixNode, MixNode.Parameters.Mix, newMix);
                Mix = newMix;
            }
        }
    }
}
