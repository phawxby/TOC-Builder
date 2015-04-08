using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Submission_TOC_Builder.Builders
{
    /// <summary>
    /// Interface class for builders should more be needed in future
    /// </summary>
    interface IBuilder
    {
        /// <summary>
        /// Sets the current directory for the builder
        /// </summary>
        /// <param name="path"></param>
        void SetDir(String path);
        /// <summary>
        /// Sets the current directory for the builder
        /// </summary>
        /// <param name="dir"></param>
        void SetDir(DirectoryInfo dir);

        /// <summary>
        /// The number of files found that will be processed by the builder
        /// </summary>
        Int32 FoundFiles { get; }

        /// <summary>
        /// Is the directory OK and ready to work with
        /// </summary>
        Boolean ValidDir { get; }

        /// <summary>
        /// Performs the build
        /// </summary>
        /// <returns></returns>
        Boolean Build();
    }

    /// <summary>
    /// Abstract base class for builders to provide them base functionality
    /// </summary>
    public abstract class ABuilder : IBuilder
    {
        /// <summary>
        /// The current directory the builder should work in
        /// </summary>
        protected DirectoryInfo _dir { get; set; }
        public void SetDir(string path)
        {
            if (!String.IsNullOrWhiteSpace(path))
            {
                _dir = new DirectoryInfo(path);
            }
        }

        public void SetDir(DirectoryInfo dir)
        {
            _dir = dir;
        }

        public abstract Int32 FoundFiles { get; }
        public virtual Boolean ValidDir
        {
            get
            {
                return (_dir != null && _dir.Exists);
            }
        }

        public abstract Boolean Build();
    }
}
