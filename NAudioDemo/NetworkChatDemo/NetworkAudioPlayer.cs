using System;
using NAudio.Wave;

namespace NAudioDemo.NetworkChatDemo
{
    class NetworkAudioPlayer : IDisposable
    {
        private readonly INetworkChatCodec codec;
        private readonly IAudioReceiver receiver;
        private readonly IWavePlayer waveOut;
        private readonly BufferedWaveProvider waveProvider;
        private AES256 aes;

        public NetworkAudioPlayer(INetworkChatCodec codec, IAudioReceiver receiver, AES256 aes)
        {
            this.aes = aes;
            this.codec = codec;
            this.receiver = receiver;
            receiver.OnReceived(OnDataReceived);

            waveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(codec.RecordFormat);
            waveOut.Init(waveProvider);
            waveOut.Play();
        }

        void OnDataReceived(byte[] compressed)
        {
            compressed = aes.FromAes256(compressed);
            byte[] decoded = codec.Decode(compressed, 0, compressed.Length);

            waveProvider.AddSamples(decoded, 0, decoded.Length);
        }

        public void Dispose()
        {
            receiver?.Dispose();
            waveOut?.Dispose();
        }
    }
}