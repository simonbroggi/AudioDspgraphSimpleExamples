using System;
using Unity.Audio;
using UnityEngine;

public class ScheduleParametersWave : MonoBehaviour
{
    public float Frequency = 300.0f;
    DSPGraph m_Graph;
    DSPNode m_SineWave;

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
            block.Connect(m_SineWave, 0, m_Graph.RootDSP, 0);
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
        }
    }

    void OnGUI()
    {
        using (var block = m_Graph.CreateCommandBlock())
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(100, 70, 300, 30), "Frequency:");
            var newFrequency = GUI.HorizontalSlider(new Rect(100, 100, 300, 30), Frequency, 200.0f, 2000.0f);
            if (Math.Abs(newFrequency - Frequency) > 0.01f)
            {
                block.SetFloat<SinWaveNode.Parameters, SinWaveNode.Providers, SinWaveNode>(m_SineWave, SinWaveNode.Parameters.Frequency, newFrequency);
                Frequency = newFrequency;
            }
        }
    }
}
