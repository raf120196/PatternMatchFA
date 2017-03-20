namespace PatternMatchFA
{
    class Transition
    {
        StateOfFA startState = null;
        StateOfFA finalState = null;
        public Transition()
        {
            this.startState = new StateOfFA();
            this.finalState = new StateOfFA();
        }

        public Transition(StateOfFA startState, StateOfFA finalState)
        {
            this.startState = startState;
            this.finalState = finalState;
        }

        public StateOfFA GetStartState()
        {
            return startState;
        }

        public StateOfFA GetFinalState()
        {
            return finalState;
        }
    }
}
