﻿using System.Diagnostics;
using System.Globalization;

namespace Octokit
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ProjectColumnMove
    {
        public ProjectColumnMove() { }

        public ProjectColumnMove(ProjectColumnPosition position, int? columnId)
        {
            switch (position)
            {
                case ProjectColumnPosition.After:
                    Ensure.ArgumentNotNull(columnId, "columnId");
                    Position = string.Format("after:{0}", columnId);
                    break;
                case ProjectColumnPosition.First:
                    Position = "first";
                    break;
                case ProjectColumnPosition.Last:
                    Position = "last";
                    break;
            }
        }

        public string Position { get; private set; }

        internal string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "Position: {0}", Position);
            }
        }
    }

    public enum ProjectColumnPosition
    {
        First,
        Last,
        After
    }
}
