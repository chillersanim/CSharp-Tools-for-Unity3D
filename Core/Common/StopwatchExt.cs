using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UnityTools.Core
{
    public sealed class StopwatchExt : IEnumerable<StopwatchExt.Checkpoint>
    {
        private readonly Stopwatch stopwatch;

        private readonly List<Checkpoint> checkpoints;

        private StringBuilder sb;

        public TimeSpan Elapsed => this.stopwatch.Elapsed;

        public long ElapsedMilliseconds => this.stopwatch.ElapsedMilliseconds;

        public long ElapsedTicks => this.stopwatch.ElapsedTicks;

        public bool IsRunning => this.stopwatch.IsRunning;

        public StopwatchExt()
        {
            this.stopwatch = new Stopwatch();
            this.checkpoints = new List<Checkpoint>();
        }

        public void Start()
        {
            this.stopwatch.Start();
        }

        public void Stop()
        {
            this.stopwatch.Stop();
        }

        public void Reset()
        {
            this.stopwatch.Reset();
            this.checkpoints.Clear();
        }

        public void Restart()
        {
            this.Stop();
            this.Reset();
            this.Start();
        }

        public void RecordCheckpoint(string name)
        {
            if (!this.stopwatch.IsRunning)
            {
                this.checkpoints.Add(new Checkpoint(name, this.stopwatch.Elapsed, this.stopwatch.ElapsedMilliseconds, this.stopwatch.ElapsedTicks));
                return;
            }

            this.stopwatch.Stop();
            this.checkpoints.Add(new Checkpoint(name, this.stopwatch.Elapsed, this.stopwatch.ElapsedMilliseconds, this.stopwatch.ElapsedTicks));
            this.stopwatch.Start();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (this.checkpoints.Count == 0)
            {
                return "No checkpoints";
            }

            if (this.sb == null)
            {
                this.sb = new StringBuilder();
            }

            this.sb.Clear();

            var maxNameLength = 0;
            var maxValueLength = 8;

            var prev = new Checkpoint();

            foreach (var checkpoint in this.checkpoints)
            {
                var elapsedMs = checkpoint.Elapsed.TotalMilliseconds - prev.Elapsed.TotalMilliseconds;

                maxNameLength = Math.Max(maxNameLength, checkpoint.Name?.Length ?? 0);
                maxValueLength = Math.Max(maxValueLength, $"{elapsedMs:0.0000} ms".Length);

                prev = checkpoint;
            }

            maxValueLength = Math.Max(maxValueLength, $"{this.stopwatch.Elapsed.TotalMilliseconds:0.0000} ms".Length);

            prev = new Checkpoint();

            this.sb.Append("Name");
            this.sb.Append(' ', maxNameLength - "Name".Length);
            this.sb.Append("   |  ");
            this.sb.Append("Elapsed");
            this.sb.AppendLine();

            this.sb.Append('-', maxNameLength);
            this.sb.Append("---+--");
            this.sb.Append('-', maxValueLength);
            this.sb.AppendLine();

            for (var i = 0; i < this.checkpoints.Count; i++)
            {
                var checkpoint = this.checkpoints[i];
                var elapsedMs = checkpoint.Elapsed.TotalMilliseconds - prev.Elapsed.TotalMilliseconds;
                var elapsedText = $"{elapsedMs:0.0000} ms";

                this.sb.Append(checkpoint.Name);
                this.sb.Append(':');
                this.sb.Append(' ', maxNameLength - checkpoint.Name?.Length ?? 0);
                this.sb.Append("  |  ");
                this.sb.Append(' ', maxValueLength - elapsedText.Length);
                this.sb.Append(elapsedText);
                this.sb.AppendLine();

                prev = checkpoint;
            }

            this.sb.Append(' ', maxNameLength);
            this.sb.Append("   |");
            this.sb.AppendLine();

            var totalElapsedText = $"{this.stopwatch.Elapsed.TotalMilliseconds:0.0000} ms";
            this.sb.Append("Total:");
            this.sb.Append(' ', maxNameLength - "Total:".Length);
            this.sb.Append("   |  ");
            this.sb.Append(' ', maxValueLength - totalElapsedText.Length);
            this.sb.Append(totalElapsedText);
            this.sb.AppendLine();

            return this.sb.ToString();
        }

        public readonly struct Checkpoint
        {
            public readonly string Name;

            public readonly TimeSpan Elapsed;

            public readonly long ElapsedMilliseconds;

            public readonly long ElapsedTicks;

            public Checkpoint(string name, TimeSpan elapsed, long elapsedMilliseconds, long elapsedTicks)
            {
                this.Name = name.Trim();
                this.Elapsed = elapsed;
                this.ElapsedMilliseconds = elapsedMilliseconds;
                this.ElapsedTicks = elapsedTicks;
            }
        }

        /// <inheritdoc />
        public IEnumerator<Checkpoint> GetEnumerator()
        {
            return this.checkpoints.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
