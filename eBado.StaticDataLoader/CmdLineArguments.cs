using CmdLine;

namespace eBado.StaticDataLoader
{
    /// <summary>
    /// The list of specified arguments for Static Data Loader
    /// </summary>
    [CommandLineArguments(Program = "Static Data Loader", Title = "eBado Data Loader", Description = "Commands for loader")]
    public class CmdLineArguments
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CmdLineArguments"/> is stations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if stations; otherwise, <c>false</c>.
        /// </value>
        [CommandLineParameter(Command = "CATEGORIES", Default = false, Description = "Run Main Categories Data Import", Name = "Categories")]
        public bool Categories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CmdLineArguments" /> is RCS.
        /// </summary>
        /// <value>
        ///   <c>true</c> if RCS; otherwise, <c>false</c>.
        /// </value>
        [CommandLineParameter(Command = "SUBCATEGORIES", Default = false, Description = "Run Sub-Categories Data Import", Name = "SubCategories")]
        public bool SubCategories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="CmdLineArguments" /> is [all].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [all]; otherwise, <c>false</c>.
        /// </value>
        [CommandLineParameter(Command = "ALL", Default = false, Description = "Run All Data Import", Name = "All")]
        public bool All { get; set; }
    }
}
