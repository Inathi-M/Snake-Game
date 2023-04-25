using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState
    {
        public int Rows { get; }
        public int Columns { get; }
        public GridValue[,] Grid { get; }
        public Direction Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public GameState(int Row, int Column) {
            int Rows = Row;
            int Columns = Column;
            Grid = new GridValue[Rows, Columns];
            Dir = Direction.Right;
            AddSnake();
            AddFood();
        }
        //This method is used to add the snake to the Grid
        private void AddSnake() {
            int r = Rows / 2;

            //This will be the initial positon of the snake
            for (int c =1;c<= 3;c++)
            {
                Grid[c, r] = GridValue.Snake;
                snakePositions.AddFirst(new Position(r, c));

            }
        }

        //This method returns all empty Grid positions
        private IEnumerable<Position> EmptyPositions() {

            for (int r = 0; r < Rows;r++) {
                for (int c =0; c < Columns;c++) {
                    if (Grid[r,c] == GridValue.Empty) { 
                        yield return new Position(r,c);
                    }
                }
            }
        }

        //This method will add the food of the snake
        private void AddFood() {
            List<Position> empty = new List<Position>(EmptyPositions());

            if (empty.Count == 0) { 
                return;
            }
            //Puts the food in a random position
            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Column] =  GridValue.Food;

        }

        //Returns postion of the snake head
        public Position HeadPosition() {
            return snakePositions.First.Value;
        }

        public Position TailPosition() {
            return snakePositions.Last.Value;        
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions; 
        }
        //This method adds the head of the snake
        private void AddHead(Position pos) {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Column] = GridValue.Snake;
        }
        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = GridValue.Empty;
            snakePositions.RemoveLast();
        }

        public void ChangeDirection(Direction dir) {
            Dir = dir;
            
        }

        //This method checks if the given position is outside the grid or not 
        private bool OutsideGrid(Position pos) {
            return pos.Row < 0 || pos.Row >= Rows || pos.Column < 0 || pos.Column >= Columns;
        }

        //This method checks what the snake will hit if it moves
        private GridValue WillHit(Position newHeadPos) {
            if (OutsideGrid(newHeadPos)) {
                return GridValue.Outside;
            }
            return Grid[newHeadPos.Row, newHeadPos.Column];

        }
        //This method will literally move the snake start with the head position
        public void Move() {
            Position newHeadPos = HeadPosition().Translate(Dir);
            GridValue hit = WillHit(newHeadPos);

            if (hit == GridValue.Outside || hit == GridValue.Snake)
            {
                GameOver = true;
            } else if (hit == GridValue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);

            } else if (hit == GridValue.Food) { 
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }


        }


    }
}
