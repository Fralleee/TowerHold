using UnityEngine;
using System.IO;

public class AudioRecorder : MonoBehaviour
{
	FileStream _fileStream;
	BinaryWriter _writer;
	bool _isRecording;
	string _outputPath;
	readonly object _lockObject = new object();
	byte[] _buffer;

	public int SampleRate { get; private set; }
	public int Channels { get; private set; }

	void Start()
	{
		SampleRate = AudioSettings.outputSampleRate;
		Channels = GetChannelCount(AudioSettings.speakerMode);
	}

	public void StartRecording(string outputPath)
	{
		_outputPath = outputPath;
		_fileStream = new FileStream(_outputPath, FileMode.Create);
		_writer = new BinaryWriter(_fileStream);
		WriteWavHeader();
		_isRecording = true;
		Debug.Log("Audio recording started.");
	}

	public void StopRecording()
	{
		lock (_lockObject)
		{
			_isRecording = false;
			WriteWavFooter();
			if (_writer != null)
			{
				_writer.Close();
				_writer = null;
			}

			if (_fileStream != null)
			{
				_fileStream.Close();
				_fileStream = null;
			}
		}
		Debug.Log("Audio recording stopped.");
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
		if (!_isRecording)
		{
			return;
		}

		var dataSize = data.Length * sizeof(short);
		if (_buffer == null || _buffer.Length < dataSize)
		{
			_buffer = new byte[dataSize];
		}

		lock (_lockObject)
		{
			for (var i = 0; i < data.Length; i++)
			{
				var pcmSample = (short)Mathf.Clamp(data[i] * 32767f, -32768f, 32767f);
				_buffer[i * 2] = (byte)(pcmSample & 0xFF);
				_buffer[(i * 2) + 1] = (byte)((pcmSample >> 8) & 0xFF);
			}

			_writer.Write(_buffer);
		}
	}

	void WriteWavHeader()
	{
		// RIFF header
		_writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
		_writer.Write(0); // Placeholder for file size
		_writer.Write(new char[4] { 'W', 'A', 'V', 'E' });

		// fmt subchunk
		_writer.Write(new char[4] { 'f', 'm', 't', ' ' });
		_writer.Write(16); // Subchunk1Size (16 for PCM)
		_writer.Write((short)1); // AudioFormat (1 for PCM)
		_writer.Write((short)Channels); // NumChannels
		_writer.Write(SampleRate); // SampleRate
		_writer.Write(SampleRate * Channels * sizeof(short)); // ByteRate
		_writer.Write((short)(Channels * sizeof(short))); // BlockAlign
		_writer.Write((short)(16)); // BitsPerSample

		// data subchunk
		_writer.Write(new char[4] { 'd', 'a', 't', 'a' });
		_writer.Write(0); // Placeholder for data size
	}

	void WriteWavFooter()
	{
		// Update file size and data chunk size
		_writer.Seek(4, SeekOrigin.Begin);
		_writer.Write((int)(_fileStream.Length - 8));

		_writer.Seek(40, SeekOrigin.Begin);
		_writer.Write((int)(_fileStream.Length - 44));
	}

	int GetChannelCount(AudioSpeakerMode mode)
	{
#pragma warning disable IDE0072 // Add missing cases
		return mode switch
		{
			AudioSpeakerMode.Mono => 1,
			AudioSpeakerMode.Stereo => 2,
			AudioSpeakerMode.Quad => 4,
			AudioSpeakerMode.Surround => 5,
			AudioSpeakerMode.Mode5point1 => 6,
			AudioSpeakerMode.Mode7point1 => 8,
			AudioSpeakerMode.Prologic => 2,
			_ => 2
		};
#pragma warning restore IDE0072 // Add missing cases
	}
}
