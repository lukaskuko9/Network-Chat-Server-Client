using System;

namespace Server
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    class CommandAttribute : Attribute
    {
        public string CommandTrigger { get; set; }
        public CommandAttribute(string cmdTrigger)
        {
            this.CommandTrigger = cmdTrigger;
            if (!this.CommandTrigger.EndsWith(" "))
                this.CommandTrigger += " ";
        }
    }
}
