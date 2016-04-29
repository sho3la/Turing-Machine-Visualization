using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineSimulation
{
    class TuringMachine
    {
        public Tape tape;
        public List<State> states;
        private State curState;


       public  List<string> trans = new List<string>();



        LoadFile file = new LoadFile();

        public TuringMachine()
        {
            tape = new Tape();
            states = new List<State>();
        }


        public List<State> runTuringMachine(string text)
        {
            List<State> ret;
            ret = new List<State>();
            curState = states[0];

            List<char> str = new List<char>();

            str.Add('#');
            str.Add('#');
            for (int i = 0; i < text.Length; ++i)
            {
                str.Add(text[i]);
            }
            str.Add('#');
            str.Add('#');


            int head = 2;

            while (!curState.isFinal)
            {

                //search by key..
                char key = str[head];
                if (curState.transition.ContainsKey(key))
                {
                    curState.transition.TryGetValue(key, out curState.tmp);

                    //update the string..
                    str[head] = curState.tmp.Item1;

                    //update head
                    head = moveDirection(curState.tmp.Item2, head);


                    trans.Add(GetTransetion(curState));

                    //new state..
                    ret.Add(curState);
                    curState = curState.tmp.Item3;


                }
                else
                {
                    break;
                }
            }
            ret.Add(curState);



            return ret;
        }



        /// <summary>
        /// load file to memory
        /// </summary>
        /// <param name="fileDir"></param>

        public void load(string fileDir)
        {
            file.fileLoad(fileDir);

            int prevstate = -1;


            //set states
            for (int i = 0; i < file.commands.Count; ++i)
            {
                string tmp = file.commands[i];

                int t = Convert.ToInt32(splite(tmp)[2]);

                if (Convert.ToInt32(splite(tmp)[0]) != prevstate)
                    states.Add(new State(Convert.ToInt32(splite(tmp)[0]), false));

                prevstate = Convert.ToInt32(splite(tmp)[0]);
            }

            //set final state out..
            states.Add(new State(prevstate + 1, true));



            //add transitions
            for (int i = 0; i < states.Count; ++i)
            {
                for (int j = 0; j < file.commands.Count; ++j)
                {
                    string command = file.commands[j];

                    if (states[i].id == Convert.ToInt32(splite(command)[0]))
                    {
                        char read = char.Parse(splite(command)[1]);
                        char write = char.Parse(splite(command)[3]);
                        State.dir dir = moveDirection(splite(command)[4]);

                        int index = Convert.ToInt32(splite(command)[2]);
                        for (int x = 0; x < states.Count; ++x)
                        {
                            if (index == states[x].id)
                            {
                                index = x;
                                break;
                            }
                        }
                        State next = states[index];

                        states[i].addTransition(read, write, dir, next);
                    }
                }
            }


        }

        string[] splite(string str)
        {
            string[] words = str.Split(' ');

            return words;
        }

        State.dir moveDirection(string str)
        {
            if (str == "L")
            {
                return State.dir.L;
            }
            if (str == "R")
            {
                return State.dir.R;
            }
            if (str == "S")
            {
                return State.dir.S;
            }

            return 0;
        }

        int moveDirection(State.dir str, int index)
        {
            if (str == State.dir.L)
            {
                index--;
            }
            if (str == State.dir.R)
            {
                index++;
            }
            if (str == State.dir.S)
            {

            }

            return index;

        }

        int checkTarget(State s, string str)
        {
            if (s.isFinal)
            {
                return s.id;
            }
            else
                return Convert.ToInt32(splite(str)[2]);

        }


        public string GetTransetion(State s)
        {
            string res = null;

            var key = s.transition.FirstOrDefault(x => x.Value == s.tmp).Key;

            res = string.Concat(res, key);
            res = string.Concat(res, "/");
            res = string.Concat(res, s.tmp.Item1);
            res = string.Concat(res, ",");
            res = string.Concat(res, s.tmp.Item2);
            res = string.Concat(res, ".");

            return res;
        }
    }
}

