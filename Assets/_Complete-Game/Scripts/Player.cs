using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

namespace Completed
{
	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		
		private int HpPlayer = 3;                           //Used to store player Hp total during level.
        public int enemyDamage = 1;                     //The amount of Hp to subtract from the player when attacking.


        //Start overrides the Start function of MovingObject
        protected override void Start ()
		{
			
			//Get the current Hp point total stored in GameManager.instance between levels.
			HpPlayer = GameManager.instance.playerHp;

            //Call the Start function of the MovingObject base class.
            base.Start ();
		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local Hp total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerHp = HpPlayer;
		}
		
		
		private void Update ()
		{
			//If it's not the player's turn, exit the function.
			if(!GameManager.instance.playersTurn) return;
			
			int horizontal = 0;  	//Used to store the horizontal move direction.
			int vertical = 0;		//Used to store the vertical move direction.
			
			//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//Check if moving horizontally, if so set vertical to zero.
			if(horizontal != 0)
			{
				vertical = 0;
			}
			//Check if we have a non-zero value for horizontal or vertical
			if(horizontal != 0 || vertical != 0)
			{
				//Call AttemptMove passing in the generic parameter Enemy, since that is what Player may interact with if they encounter one (by attacking it)
				//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
				AttemptMove<Enemy> (horizontal, vertical);
			}
		}
		
		//AttemptMove overrides the AttemptMove function in the base class MovingObject
		//AttemptMove takes a generic parameter T which for Player will be of the type Enemy, it also takes integers for x and y direction to move in.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			
			//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
			base.AttemptMove <T> (xDir, yDir);
			
			
			//Set the playersTurn boolean of GameManager to false now that players turn is over.
			GameManager.instance.playersTurn = false;
		}
		
		
		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		protected override void OnCantMove <T> (T component)
		{
            //Declare hitPlayer and set it to equal the encountered component.
            Enemy hitEnemy = component as Enemy;

            //Call the LoseFood function of hitPlayer passing it playerDamage, the amount of foodpoints to be subtracted.
            hitEnemy.LoseEnemyHp (enemyDamage);
        }
		
		
		//OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Check if the tag of the trigger collided with is Exit.
			if(other.tag == "Exit")
			{
                SceneManager.LoadScene("RamsescapeVictoryScene");
			}
		}
		
		
		//LoseHp is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseHp (int loss)
		{
			
			//Subtract lost Hp points from the players total.
			HpPlayer -= loss;
			
			//Update the Hp display with the new total.
			//HpText.text = "-"+ loss + " Hp: " + HpPlayer;
			
			//Check to see if game has ended.
			CheckIfGameOver ();
		}
		
		
		//CheckIfGameOver checks if the player is out of Hp points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if Hp point total is less than or equal to zero.
			if (HpPlayer <= 0) 
			{
				//Call the GameOver function of GameManager.
				GameManager.instance.GameOver ();
			}
		}
	}
}

