using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;

namespace AudioSample.OpenAI
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.AspNetCore.Http;

    public class SpeechToTextConverter
    {
        private string subscriptionKey;
        private string serviceRegion;

        public SpeechToTextConverter(string subscriptionKey, string serviceRegion)
        {
            this.subscriptionKey = subscriptionKey;
            this.serviceRegion = serviceRegion;
        }

        public async Task<string> ConvertWavToTextAsync(IFormFile file)
        {
            // Ensure the uploaded file is not null and has content
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Uploaded file is invalid.");
            }

            // Create a temporary file to store the uploaded file
            var tempFilePath = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.wav");


            // Save the uploaded file to the temporary file
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            var config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);
            var pushStream = AudioInputStream.CreatePushStream();

            // Read the uploaded file and push it to the stream
            using (var stream = file.OpenReadStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    pushStream.Write(buffer, bytesRead);
                }
                pushStream.Close();
            }

            // Configure the Speech SDK with the subscription key and service region
            // Create an audio configuration from the uploaded file stream
            using (var audioInput = AudioConfig.FromWavFileInput(tempFilePath))
            {
                try
                {


                    // Create a speech recognizer
                    using (var recognizer = new SpeechRecognizer(config, audioInput))
                    {
                        // Start the speech recognition
                        var result = await recognizer.RecognizeOnceAsync();

                        // Check the result and return the text
                        if (result.Reason == ResultReason.RecognizedSpeech)
                        {
                            return result.Text;
                        }
                        else
                        {
                            throw new Exception($"Speech recognition failed: {result.Reason} - {result.Text}");
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception($"Speech recognition failed");
                }
                }
        }
       
}
}

    public class SpeechService
    {
        private readonly string subscriptionKey;
        private readonly string region;

        public SpeechService()
        {
            subscriptionKey = "Spepch-Key";
            region = "eastus";
        }

        public async Task<string> RecognizeSpeechAsync(Stream audioStream)
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);

            // Ensure correct audio format: PCM 16-bit, 16 kHz, mono
            using (var audioInput = AudioConfig.FromStreamInput(new BinaryAudioStreamReader(audioStream)))
            using (var recognizer = new SpeechRecognizer(config, audioInput))
            {
                var result = await recognizer.RecognizeOnceAsync();
                if (result.Reason == ResultReason.RecognizedSpeech)
                {
                    return result.Text;
                }
                else if (result.Reason == ResultReason.NoMatch)
                {
                    return "No speech could be recognized.";
                }
                else if (result.Reason == ResultReason.Canceled)
                {
                  ///  var cancellation = SpeechRecognitionCancellationDetails.FromResult(result);
                   /// return $"CANCELED: Reason={cancellation.Reason}, ErrorDetails={cancellation.ErrorDetails}";
                }
            }
            return string.Empty;
        }
    }
    public class BinaryAudioStreamReader : PullAudioInputStreamCallback
    {
        private readonly BinaryReader _reader;

        public BinaryAudioStreamReader(Stream stream)
        {
            _reader = new BinaryReader(stream);
        }

        public override int Read(byte[] dataBuffer, uint size)
        {
            return _reader.Read(dataBuffer, 0, (int)size);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _reader.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    
