using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace GitHubSearch
{
    /// <summary>
    /// Responsible for showing a progressbar in the console.
    /// </summary>
    /// <remarks>
    /// Until the progress bar is finished, no other output should be written to the console.
    /// </remarks>
    internal static class ProgressIndicator
    {
        private static Task _task;

        private static int _total = 1;

        private static int _value = 0;

        private static int _progressWidth = 70;

        /// <summary>
        /// Starts the progress indicator, optionally with a specific width.
        /// </summary>
        /// <param name="width"></param>
        public static void Start(int width = 70)
        {            
            _value = 0;
            _progressWidth = width;

            Console.WriteLine(" 0%" + new string(' ', width - 2) + "100%");

            _task = Task.Run(() =>
            {
                while (_value < _total)
                {   
                    WriteProgress(_value * 100 / _total);
                    Thread.Sleep(100);
                }
                
                WriteProgress(100);

                // Keep progress bar visible for visible feedback
                // Used in case everything is cached and progress goes from 0 to 100 directly
                Thread.Sleep(300);
            });
        }

        private static void WriteProgress(int percentage)
        {
            var fillBlocks = (percentage * _progressWidth) / 100;
            var emptyBlocks = _progressWidth - fillBlocks;

            var progressString = new string('▒', fillBlocks) + new string('-', emptyBlocks);
            
            Colorful.StyleSheet styleSheet = new Colorful.StyleSheet(Color.White);
            styleSheet.AddStyle("▒", Color.Green);

            Colorful.Console.WriteStyled("\r |" + progressString + "|", styleSheet);
        }

        /// <summary>
        /// Waits until the progress indicator has reaches 100%. Typically only necessary to finish the last thread sleep.
        /// </summary>
        public static void WaitTillFinished()
        {
            _value = _total;

            if (_task != null)
            {
                _task.Wait();
                _task = null;
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        public static void SetTotal(int total)
        {
            _total = Math.Min(1, total);
        }

        public static void Step()
        {
            Interlocked.Increment(ref _value);
        }
    }
}
