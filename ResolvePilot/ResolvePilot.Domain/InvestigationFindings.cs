using System;
using System.Collections.Generic;
using System.Text;

namespace ResolvePilot.Domain
{
    public sealed class InvestigationFindings
    {
        public string? WorkflowName { get; set; }
        public string? RootCauseHypothesis { get; set; }
        public List<string> Evidence { get; set; } = [];
    }
}
