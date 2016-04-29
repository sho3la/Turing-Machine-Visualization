using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;

namespace TuringMachineSimulation
{
    public partial class GUI : Form
    {
        TuringMachine TM;
        Pen blackPen, redPen, orangePen;
        int curStateId = -1, prevStateId = -1;
        Graphics g;

        List<Point> statePosition;
        int currentStateIndex;

        //all the states of the current run (until the input text is accepted or rejected)
        List<State> allStates;

        public GUI()
        {
            InitializeComponent();
            TM = new TuringMachine();
            refreshTape();

        }

        private void initStatePositions()
        {
            statePosition = new List<Point>(TM.states.Count);
            //statePosition.Add(new Point(50, 50));
            //statePosition.Add(new Point(392, 106));
            //statePosition.Add(new Point(610, 106));
            //statePosition.Add(new Point(630, 283));
            //statePosition.Add(new Point(309, 332));
            //statePosition.Add(new Point(626, 435));
            //statePosition.Add(new Point(94, 336));
            //statePosition.Add(new Point(800, 114));
            //statePosition.Add(new Point(793, 435));



            for (int i = 0; i < TM.states.Count; ++i)
            {
                statePosition.Add(new Point(50 + (i * 55), 50));
            }


        }

        public void refreshTape()
        {
            TapeTextBox.Text = TM.tape.getTapeState();
            TapeTextBox.SelectionStart = TM.tape.getCurrentPosition();
            TapeTextBox.SelectionLength = 1;
            TapeTextBox.SelectionColor = Color.Red;
            TapeTextBox.SelectionBackColor = Color.Yellow;
        }


        private void GraphicalTMPanel_Paint(object sender, PaintEventArgs e)
        {
            blackPen = new Pen(Color.FromArgb(0, 0, 0));
            redPen = new Pen(Color.Red);
            orangePen = new Pen(Color.Orange);
            g = GraphicalTMPanel.CreateGraphics();

            LoadFile_Click(sender, e);


            blackPen.Width = 3F;
            redPen.Width = 5F;
            orangePen.Width = 5F;

            //drawing the states
            Point drawPosition;
            for (int i = 0; i < statePosition.Count; i++)
            {
                drawPosition = statePosition[i];
                drawPosition.X -= 25;
                drawPosition.Y -= 25;
                if (i == curStateId)
                    drawState(i, redPen, TM.states[i].isFinal);
                else if (i == prevStateId)
                    drawState(i, orangePen, TM.states[i].isFinal);
                else
                    drawState(i, blackPen, TM.states[i].isFinal);
            }

            //drawing the labels of the nodes
            for (int i = 0; i < statePosition.Count; i++)
            {
                Label lbl = new Label();
                lbl.Size = new Size(25, 15);
                lbl.Text = "Q" + i.ToString();
                Point lblPosition = statePosition[i];
                lblPosition.X -= 10;
                lblPosition.Y -= 5;
                lbl.Location = lblPosition;
                GraphicalTMPanel.Controls.Add(lbl);
            }



        }

        private void drawState(int stateID, Pen pen, bool isfinal)
        {
            Point drawPosition = statePosition[stateID];
            drawPosition.X -= 25;
            drawPosition.Y -= 25;
            g.DrawEllipse(pen, new Rectangle(drawPosition, new Size(50, 50)));

            //double line for final states
            if (isfinal)
            {
                drawPosition.X += 5;
                drawPosition.Y += 5;
                pen = new Pen(pen.Color, 3);
                g.DrawEllipse(pen, new Rectangle(drawPosition, new Size(40, 40)));
            }
        }

        private void GraphicalTMPanel_Clicked(object sender, MouseEventArgs e)
        {
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            resultTextBox.Clear();
            resultTextBox.BackColor = Color.LightGray;


            TM.tape = new Tape();
            TM.tape.setTapeState(inputTextBox.Text);
            refreshTape();

            allStates = TM.runTuringMachine(inputTextBox.Text);

            TTransition.Text = TM.trans[0];

            currentStateIndex = 0;
            curStateId = prevStateId = allStates[0].id;
        }

        private void NextStepButton_Click(object sender, EventArgs e)
        {
            if (allStates != null)
            {
                

                if (currentStateIndex < allStates.Count - 1)
                {

                    Tuple<char, State.dir, State> currentTransition = allStates[currentStateIndex].transition[TapeTextBox.Text[TM.tape.getCurrentPosition()] == '$' ? ' ' : TapeTextBox.Text[TM.tape.getCurrentPosition()]];


                    TM.tape.replaceCell(currentTransition.Item1 == ' ' ? '$' : currentTransition.Item1);

                    if (currentTransition.Item2 == State.dir.L)
                        TM.tape.goLeft();
                    else if (currentTransition.Item2 == State.dir.R)
                        TM.tape.goRight();

                    refreshTape();

                    drawState(prevStateId, blackPen, allStates[currentStateIndex].isFinal);
                    prevStateId = curStateId;
                    curStateId = allStates[currentStateIndex + 1].id;
                    drawState(prevStateId, orangePen, allStates[currentStateIndex].isFinal);
                    drawState(curStateId, redPen, allStates[currentStateIndex + 1].isFinal);
                    currentStateIndex++;

                    //trasnitions out
                    if (currentStateIndex < allStates.Count - 1)
                    {
                        TTransition.Text = TM.trans[currentStateIndex];
                    }

                    if (currentStateIndex == allStates.Count - 1)
                    {
                        if (allStates[currentStateIndex].isFinal)
                        {
                            resultTextBox.ForeColor = Color.GreenYellow;
                            resultTextBox.BackColor = Color.Green;
                            resultTextBox.Text = "Accepted";
                        }
                        else
                        {
                            resultTextBox.ForeColor = Color.Pink;
                            resultTextBox.BackColor = Color.Red;
                            resultTextBox.Text = "Rejected";
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("you had to enter string and submit !!");
            }



        }

        private void CompleteButton_Click(object sender, EventArgs e)
        {
            if (allStates != null)
            {
                while (currentStateIndex < allStates.Count - 1)
                {
                    NextStepButton_Click(sender, e);
                    TapeTextBox.Refresh();
                    System.Threading.Thread.Sleep(1000);
                }
            }
            else
            {
                MessageBox.Show("you had to enter string and submit !!");
            }
        }

        private void goToFinalButton_Click(object sender, EventArgs e)
        {
            if (allStates != null)
            {
                while (currentStateIndex < allStates.Count - 1)
                {
                    NextStepButton_Click(sender, e);
                }
                TapeTextBox.Refresh();
            }
            else
            {
                MessageBox.Show("you had to enter string and submit !!");
            }
        }

        private void LoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.InitialDirectory = "Desktop/teoryTaskData";
            string fileDire = null;

            if (file.ShowDialog() == DialogResult.OK)
            {
                fileDire = file.FileName;

                TM.load(fileDire);

                //saving the nodes locations
                initStatePositions();


            }
        }



    }
}
