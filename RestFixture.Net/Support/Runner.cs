namespace restFixture.Net.Support
{
    /// <summary>
    /// What runner this table is running on.
    /// 
    /// Note, the OTHER runner is primarily for testing purposes.
    /// 
    /// </summary>
    public enum Runner
    {
        /// <summary>
        /// the slim runner
        /// </summary>
        SLIM,

        /// <summary>
        /// the fit runner
        /// </summary>
        FIT,

        /// <summary>
        /// any other runner
        /// </summary>
        OTHER
    }
}