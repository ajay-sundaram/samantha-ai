using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace samantha_ai
{
    class Program
    {
        private static readonly HttpClient Samantha = new HttpClient();
        private static Samantha client = new Samantha();
        private static string input_text = string.Empty;
        private const string bot_name = Bot.Doctor;
        private static SpeechConfig speechConfig = SpeechConfig.FromSubscription("b70f3f978c1543f49d59bf8a1b3c0028", "eastus");
        private static string BotVoice = Voice.Christopher;
        static async Task Main(string[] args)
        {

            //bot config
            speechConfig.EnableDictation();
            speechConfig.SpeechSynthesisVoiceName = string.IsNullOrWhiteSpace(BotVoice) ? Voice.Christopher : BotVoice;

            //Bot Introduction
            Console.WriteLine("*****************************************************************");
            Console.WriteLine($"********** Welcome to the AI Chat with {bot_name}******************");
            Console.WriteLine("*****************************************************************");
            Console.WriteLine("Type exit to quit chat. ");
            await SpeakOut($"Hello!. I am {bot_name}. I can be a good companion.");
            Console.WriteLine();

            //User Intorduction
            string user_name = string.Empty;
            await SpeakOut("Say your Name:");
            Console.Write("Say your Name: ");
            user_name = await ListenFromMic();
            Console.WriteLine($"{bot_name.ToUpper()}: Hello {user_name}!");
            await SpeakOut($" Hello {user_name}! Nice to meet you.");
            Console.WriteLine();

            //Listen and Interact
            while (input_text != "exit")
            {
                Console.Write($"{user_name.ToUpper()}: ");
                input_text = await ListenFromMic();
                string formatted_text = input_text.Replace(' ', '+');
                string bot_reply = await client.BotReplyText(formatted_text);
                Console.WriteLine($"{bot_name.ToUpper()}: {bot_reply}");
                await SpeakOut(bot_reply);
                Console.WriteLine();
            }
        }

        #region Speech to Text
        async static Task<string> ListenFromMic()
        {
            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            var recognizer = new Microsoft.CognitiveServices.Speech.SpeechRecognizer(speechConfig, audioConfig);
            var stopRecognition = new TaskCompletionSource<int>();
            string recognized_text = string.Empty;

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                   
                    Console.WriteLine($" --> {e.Result.Text}");
                    recognized_text = e.Result.Text;

                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the speech key and location/region info?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStopped += (s, e) =>
            {
              stopRecognition.TrySetResult(0);
            };

            recognizer.SpeechStartDetected += (s, e) =>
            {
                Console.Write("(Listening)");
            };

           

            await recognizer.RecognizeOnceAsync();

            // Waits for completion. Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            return recognized_text;

        }
        #endregion


        #region Text to Speech
        async static Task SpeakOut(string textInput)
        {
            var synthesizer = new SpeechSynthesizer(speechConfig);
            await synthesizer.SpeakTextAsync(textInput);
        }
        #endregion

    }
}

