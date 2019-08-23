namespace GRA.Domain.Model
{
    /// <summary>
    /// A JobStatus object communicates ongoing status or final disposition of a job back to the
    /// front-end job runner. Communicated via WebSocket (as JSON), the JavaScript front-end either
    /// configures options or takes action based on the information passed in a JobStatus.
    /// </summary>
    public class JobStatus
    {
        /// <summary>
        /// Title of the JavaScript modal window to display, if unset the title stays what it was
        /// previously
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Percent complete to show in the activity bar. Should be between 0 and 100. If unset, 
        /// percent remains what it was previously.
        /// </summary>
        public int? PercentComplete { get; set; }

        /// <summary>
        /// Status text to show below the activity bar, if unset the status text remains the same
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Signal that the job has completed and alter the JavaScript modal window as such.
        /// </summary>
        public bool Complete { get; set; }

        /// <summary>
        /// Only consulted if Complete is true - indicate in the JavaScript modal window that the
        /// job completed with one or more errors.
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// URL to navigate to once the job has completed. Once set, the value remains the same
        /// unless it is set again and sent to the client.
        /// </summary>
        public string SuccessUrl { get; set; }

        /// <summary>
        /// If true, automatically redirect to the SuccessUrl upon job success, otherwise allow
        /// the user to read the final Status before clicking a button to navigate. Once set, the
        /// value remains the same unless it is set again and sent to the client.
        /// </summary>
        public bool? SuccessRedirect { get; set; }

        /// <summary>
        /// URL to navigate to if the job is cancelled. Once set, the value remains the same
        /// unless it is set again and sent to the client.
        /// </summary>
        public string CancelUrl { get; set; }
    }
}
