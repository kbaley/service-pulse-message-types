using System;
using System.Collections.Generic;

namespace Shared.Events;

public interface IAgentTerminalListWasExported
{
    DateTime BetDate { get; set; }

    List<AgentTerminal> AgentTerminals { get; set; }
}
