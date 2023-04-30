﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new Dictionary<GridValue, ImageSource>()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food }

        };

        private readonly int rows = 15, columns = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;



        public MainWindow()
        {

            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, columns);
        }

        private async Task RunGame() 
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e) 
        {
            if (Overlay.Visibility == Visibility.Visible) 
            { 
                e.Handled = true;
            }

            if (!gameRunning) 
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) 
        {
            if (gameState.GameOver) 
            {
                return;
            }

            switch (e.Key) {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;  
            }
        }

        private async Task GameLoop() 
        {
            while (!gameState.GameOver) 
            { 
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid() {
            Image[,] images = new Image[rows, columns];
            GameGrid.Rows = rows;
            GameGrid.Columns = columns;

            for (int r = 0; r < rows;r++) 
            {
                for (int c = 0; c < columns;c++) 
                {
                    Image image = new Image
                    {
                        Source = Images.Empty
                    };

                    images[r,c] = image;
                    GameGrid.Children.Add(image);

                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            ScoreText.Text = $"SCORE {gameState.Score}";
        }

        private void DrawGrid() 
        {
            for (int r = 0;r < rows ;r++) 
            {
                for (int c = 0; c < columns;c++) 
                {
                    GridValue gridval = gameState.Grid[r,c];
                    gridImages[r,c].Source = gridValToImage[gridval];
                }
            }
        }

        //This method counts down the clock to the game starting
        private async Task ShowCountDown() 
        {
            for (int i=3; i>= 1;i-- ) 
            { 
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }        
        }

        //This method will help to restart the game
        private async Task ShowGameOver() 
        { 
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }
    }
}
