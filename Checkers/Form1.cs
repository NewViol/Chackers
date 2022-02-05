using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Checkers
{

   
    public partial class Form1 : Form
    {
        const int MapSize = 8;
        const int SquareSize = 60;

        int PresentPlayer;

        List<Button> SimpleSteps = new List<Button>();

        int dopPossibleSteps = 0;
        Button PrevButton;
        Button PressedButton;
        bool isContinue = false;

        bool isMoving;

        int[,] map = new int[MapSize, MapSize];

        Button[,] buttons = new Button[MapSize, MapSize];

        Image WhiteFigure;
        Image BlackFigure;
        public Form1()
        {
            InitializeComponent();
            BackColor = Color.DarkViolet;

            WhiteFigure = new Bitmap(new Bitmap("chec2323kers_PNG35.png"), new Size(SquareSize - 10, SquareSize - 10));
            BlackFigure = new Bitmap(new Bitmap("check3232ers_PNG36.png"), new Size(SquareSize - 10, SquareSize - 10));

            this.Text = "Checkers";

            MapItem();
        }

        public void MapItem()
        {

            PresentPlayer = 1;
            isMoving = false;
            PrevButton = null;

            map = new int[MapSize, MapSize] {
                { 0,2,0,2,0,2,0,2},
                { 2,0,2,0,2,0,2,0},
                { 0,2,0,2,0,2,0,2},
                { 0,0,0,0,0,0,0,0},
                { 0,0,0,0,0,0,0,0},
                { 1,0,1,0,1,0,1,0},
                { 0,1,0,1,0,1,0,1},
                { 1,0,1,0,1,0,1,0}
            };


            CreateMap();
        }

        public void ResetGame()
        {
            bool player1 = false;
            bool player2 = false;

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (map[i, j] == 1)
                        player1 = true;
                    if (map[i, j] == 2)
                        player2 = true;
                }
            }
            if (!player1)
            {
                MessageBox.Show("Черные выиграли", "Результат");
                this.Controls.Clear();
                MapItem();
            }
            else if(!player2)
            {
                MessageBox.Show("Белые выиграли", "Результат");
                this.Controls.Clear();
                MapItem();
            }

        }
        public void CreateMap()
        {
            this.Width = (MapSize) * SquareSize + 15;
            this.Height = (MapSize) * SquareSize + 40;

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    
                    Button button = new Button();
                    button.Location = new Point(j * SquareSize, i * SquareSize);
                    button.Size = new Size(SquareSize, SquareSize);
                    button.Click += new EventHandler(OnFigurePress);
                    if(map[i,j] == 1)
                    {
                        button.Image = WhiteFigure;
                    }
                    else if(map[i,j] ==2)
                    {
                        button.Image = BlackFigure;
                    }

                    button.BackColor = GetPrevButtonColor(button);
                    button.ForeColor = Color.Red;

                    buttons[i, j] = button;

                    this.Controls.Add(button);
                }
            }
        }

        public void SwitchPlayer()
        {
            PresentPlayer = PresentPlayer == 1 ? 2 : 1;
            ResetGame();
        }

        public Color GetPrevButtonColor(Button prevButton)
        {
            if ((prevButton.Location.Y / SquareSize % 2) != 0)
            {
                if ((prevButton.Location.X / SquareSize % 2) == 0)
                {
                    return Color.Gray;
                }
            }
            if ((prevButton.Location.Y / SquareSize) % 2 == 0)
            {
                if ((prevButton.Location.X / SquareSize) % 2 != 0)
                {
                    return Color.Gray;
                }
            }
            return Color.White;
        }


        public void OnFigurePress(object sender, EventArgs e)
        {
            if (PrevButton != null)
            {
                PrevButton.BackColor = GetPrevButtonColor(PrevButton);
            }
                

            PressedButton = sender as Button;

            if (map[PressedButton.Location.Y / SquareSize, PressedButton.Location.X / SquareSize] != 0 && map[PressedButton.Location.Y / SquareSize, PressedButton.Location.X / SquareSize] == PresentPlayer)
            {
                CloseSteps();
                PressedButton.BackColor = Color.Red;
                DeactivateAllButtons();
                PressedButton.Enabled = true;
                dopPossibleSteps = 0;
                if (PressedButton.Text == "KING")
                {
                    ShowSteps(PressedButton.Location.Y / SquareSize, PressedButton.Location.X / SquareSize, false);
                }
                else
                {
                    ShowSteps(PressedButton.Location.Y / SquareSize, PressedButton.Location.X / SquareSize);
                }

                if (isMoving)
                {
                    CloseSteps();
                    PressedButton.BackColor = GetPrevButtonColor(PressedButton);
                    ShowPossibleSteps();
                    isMoving = false;
                }
                else
                {
                    isMoving = true;
                }
            }
            else
            {
                if (isMoving)
                {
                    isContinue = false;
                    if (Math.Abs(PressedButton.Location.X / SquareSize - PrevButton.Location.X / SquareSize) > 1)
                    {
                        isContinue = true;
                        DeletePossible(PressedButton, PrevButton);
                    }
                    int temp = map[PressedButton.Location.Y / SquareSize, PressedButton.Location.X / SquareSize];
                    map[PressedButton.Location.Y / SquareSize, PressedButton.Location.X / SquareSize] = map[PrevButton.Location.Y / SquareSize, PrevButton.Location.X / SquareSize];
                    map[PrevButton.Location.Y / SquareSize, PrevButton.Location.X / SquareSize] = temp;
                    PressedButton.Image = PrevButton.Image;
                    PrevButton.Image = null;
                    PressedButton.Text = PrevButton.Text;
                    PrevButton.Text = "";
                    SwitchButtonToCheat(PressedButton);
                    dopPossibleSteps = 0;
                    isMoving = false;
                    CloseSteps();
                    DeactivateAllButtons();
                    if (PressedButton.Text == "KING")
                    {
                        ShowSteps(PressedButton.Location.Y / SquareSize, PressedButton.Location.X / SquareSize, false);
                    }
                    else
                    {
                        ShowSteps(PressedButton.Location.Y / SquareSize, PressedButton.Location.X / SquareSize);
                    }
                    if (dopPossibleSteps == 0 || !isContinue)
                    {
                        CloseSteps();
                        SwitchPlayer();
                        ShowPossibleSteps();
                        isContinue = false;
                    }
                    else if (isContinue)
                    {
                        PressedButton.BackColor = Color.Red;
                        PressedButton.Enabled = true;
                        isMoving = true;
                    }
                }
            }

            PrevButton = PressedButton;
        }


        public void ShowPossibleSteps()
        {
            bool isDamn = true;
            bool isPossibletStep = false;
            DeactivateAllButtons();
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (map[i, j] == PresentPlayer)
                    {
                        if (buttons[i, j].Text == "KING")
                        {
                            isDamn = false;
                        }

                        else
                        {
                            isDamn = true;
                        }
                        if (IsButtonHasPossibletStep(i, j, isDamn, new int[2] { 0, 0 }))
                        {
                            isPossibletStep = true;
                            buttons[i, j].Enabled = true;
                        }
                    }
                }
            }
            if (!isPossibletStep)
            {
                ActivateAllButtons();
            }
        }
        public void SwitchButtonToCheat(Button button)
        {
            if (map[button.Location.Y / SquareSize, button.Location.X / SquareSize] == 1 && button.Location.Y / SquareSize == MapSize - 1)
            {
                button.Text = "KING";

            }
            if (map[button.Location.Y / SquareSize, button.Location.X / SquareSize] == 2 && button.Location.Y / SquareSize == 0)
            {
                button.Text = "KING";
            }
        }

        public void DeletePossible(Button endButton, Button startButton)
        {
            int count = Math.Abs(endButton.Location.Y / SquareSize - startButton.Location.Y / SquareSize);
            int startIndexX = endButton.Location.Y / SquareSize - startButton.Location.Y / SquareSize;
            int startIndexY = endButton.Location.X / SquareSize - startButton.Location.X / SquareSize;
            startIndexX = startIndexX < 0 ? -1 : 1;
            startIndexY = startIndexY < 0 ? -1 : 1;
            int currCount = 0;
            int i = startButton.Location.Y / SquareSize + startIndexX;
            int j = startButton.Location.X / SquareSize + startIndexY;
            while (currCount < count - 1)
            {
                map[i, j] = 0;
                buttons[i, j].Image = null;
                buttons[i, j].Text = "";
                i += startIndexX;
                j += startIndexY;
                currCount++;
            }

        }

        public void ShowSteps(int iCurrFigure, int jCurrFigure, bool isDamn = true)
        {
            SimpleSteps.Clear();
            ShowDiagonal(iCurrFigure, jCurrFigure, isDamn);
            if (dopPossibleSteps > 0)
            {
                CloseSimpleSteps(SimpleSteps);
            }
        }

        public void ShowDiagonal(int IcurrFigure, int JcurrFigure, bool isDamn = false)
        {
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (PresentPlayer == 2 && isDamn && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j)) break;
                }
                if (j < 7)
                {
                    j++;
                }
                else break;

                if (isDamn) break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (PresentPlayer == 2 && isDamn && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j)) break;
                }
                if (j > 0)
                {
                    j--;
                }
                else break;

                if (isDamn) break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (PresentPlayer == 1 && isDamn && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j)) break;
                }
                if (j > 0)
                {
                    j--;
                }
                else break;

                if (isDamn) break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (PresentPlayer == 1 && isDamn && !isContinue) break;
                if (IsInsideBorders(i, j))
                {
                    if (!DeterminePath(i, j)) break;
                }
                if (j < 7)
                {
                    j++;
                }
                else break;

                if (isDamn) break;
            }
        }
        public bool DeterminePath(int dopI, int dopJ)
        {

            if (map[dopI, dopJ] == 0 && !isContinue)
            {
                buttons[dopI, dopJ].BackColor = Color.Yellow;
                buttons[dopI, dopJ].Enabled = true;
                SimpleSteps.Add(buttons[dopI, dopJ]);
            }
            else
            {

                if (map[dopI, dopJ] != PresentPlayer)
                {
                    if (PressedButton.Text == "KING")
                    {
                        ShowProceduralSteps(dopI, dopJ, false);
                    }
                    else
                    {
                        ShowProceduralSteps(dopI, dopJ);
                    }
                }

                return false;
            }
            return true;
        }

        public void CloseSimpleSteps(List<Button> simpleSteps)
        {
            if (simpleSteps.Count > 0)
            {
                for (int i = 0; i < simpleSteps.Count; i++)
                {
                    simpleSteps[i].BackColor = GetPrevButtonColor(simpleSteps[i]);
                    simpleSteps[i].Enabled = false;
                }
            }
        }

        public void ShowProceduralSteps(int i, int j, bool isDamn = true)
        {
            int dirX = i - PressedButton.Location.Y / SquareSize;
            int dirY = j - PressedButton.Location.X / SquareSize;
            dirX = dirX < 0 ? -1 : 1;
            dirY = dirY < 0 ? -1 : 1;
            int dopI1 = i;
            int dopJ1 = j;
            bool isEmpty = true;
            while (IsInsideBorders(dopI1, dopJ1))
            {
                if (map[dopI1, dopJ1] != 0 && map[dopI1, dopJ1] != PresentPlayer)
                {
                    isEmpty = false;
                    break;
                }
                dopI1 += dirX;
                dopJ1 += dirY;

                if (isDamn) break;
            }
            if (isEmpty) return;
            List<Button> toClose = new List<Button>();
            bool closeSimple = false;
            int dopI2 = dopI1 + dirX;
            int dopJ2 = dopJ1 + dirY;
            while (IsInsideBorders(dopI2, dopJ2))
            {
                if (map[dopI2, dopJ2] == 0)
                {
                    if (IsButtonHasPossibletStep(dopI2, dopJ2, isDamn, new int[2] { dirX, dirY }))
                    {
                        closeSimple = true;
                    }
                    else
                    {
                        toClose.Add(buttons[dopI2, dopJ2]);
                    }
                    buttons[dopI2, dopJ2].BackColor = Color.Yellow;
                    buttons[dopI2, dopJ2].Enabled = true;
                    dopPossibleSteps++;
                }
                else break;
                if (isDamn) break;
                dopJ2 += dirY;
                dopI2 += dirX;
            }
            if (closeSimple && toClose.Count > 0)
            {
                CloseSimpleSteps(toClose);
            }

        }

        public bool IsButtonHasPossibletStep(int IcurrFigure, int JcurrFigure, bool isDamn, int[] dir)
        {
            bool PossibletStep = false;
            int j = JcurrFigure + 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (PresentPlayer == 2 && isDamn && !isContinue) break;
                if (dir[0] == 1 && dir[1] == -1 && !isDamn) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != PresentPlayer)
                    {
                        PossibletStep = true;
                        if (!IsInsideBorders(i - 1, j + 1))
                        {
                            PossibletStep = false;
                        }
                        else if (map[i - 1, j + 1] != 0)
                        {
                            PossibletStep = false;
                        }
                        else return PossibletStep;
                    }
                }
                if (j < 7)
                {
                    j++;
                }
                else break;

                if (isDamn) break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure - 1; i >= 0; i--)
            {
                if (PresentPlayer == 2 && isDamn && !isContinue) break;
                if (dir[0] == 1 && dir[1] == 1 && !isDamn) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != PresentPlayer)
                    {
                        PossibletStep = true;
                        if (!IsInsideBorders(i - 1, j - 1))
                        {
                            PossibletStep = false;
                        }
                        else if (map[i - 1, j - 1] != 0)
                        {
                            PossibletStep = false;
                        }
                        else return PossibletStep;
                    }
                }
                if (j > 0)
                {
                    j--;
                }
                else break;

                if (isDamn) break;
            }

            j = JcurrFigure - 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (PresentPlayer == 1 && isDamn && !isContinue) break;
                if (dir[0] == -1 && dir[1] == 1 && !isDamn) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != PresentPlayer)
                    {
                        PossibletStep = true;
                        if (!IsInsideBorders(i + 1, j - 1))
                        {
                            PossibletStep = false;
                        }
                        else if (map[i + 1, j - 1] != 0)
                        {
                            PossibletStep = false;
                        }
                        else return PossibletStep;
                    }
                }
                if (j > 0)
                {
                    j--;
                }
                else break;

                if (isDamn) break;
            }

            j = JcurrFigure + 1;
            for (int i = IcurrFigure + 1; i < 8; i++)
            {
                if (PresentPlayer == 1 && isDamn && !isContinue) break;
                if (dir[0] == -1 && dir[1] == -1 && !isDamn) break;
                if (IsInsideBorders(i, j))
                {
                    if (map[i, j] != 0 && map[i, j] != PresentPlayer)
                    {
                        PossibletStep = true;
                        if (!IsInsideBorders(i + 1, j + 1))
                        {
                            PossibletStep = false;
                        }
                        else if (map[i + 1, j + 1] != 0)
                        {
                            PossibletStep = false;
                        }
                        else return PossibletStep;
                    }
                }
                if (j < 7)
                {
                    j++;
                }
                else break;

                if (isDamn) break;
            }
            return PossibletStep;
        }

        public void CloseSteps()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    buttons[i, j].BackColor = GetPrevButtonColor(buttons[i, j]);
                }
            }
        }

        public bool IsInsideBorders(int dopI, int dopJ)
        {
            if (dopI >= MapSize || dopJ >= MapSize || dopI < 0 || dopJ < 0)
            {
                return false;
            }
            return true;
        }

        public void ActivateAllButtons()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    buttons[i, j].Enabled = true;
                }
            }
        }

        public void DeactivateAllButtons()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    buttons[i, j].Enabled = false;
                }
            }
        }

    }
}
