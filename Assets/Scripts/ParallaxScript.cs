using System;
using UnityEngine;

// https://blog.yarsalabs.com/parallax-effect-in-unity-2d/
namespace _Scripts
{
    public class ParallaxEffect : MonoBehaviour
    {
        private float _startingPos, //This is the starting position of the sprites.
            _lengthOfSprite; //This is the length of the sprites.
        public float AmountOfParallax; //This is amount of parallax scroll. 
        public Camera MainCamera; //Reference of the camera.



        private void Start()
        {
            //Getting the starting X position of sprite.
            _startingPos = transform.position.x;
            //Getting the length of the sprites.
            _lengthOfSprite = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        }



        private void FixedUpdate()
        {
            Vector3 Position = MainCamera.transform.position;
            float Temp = Position.x * (1 - AmountOfParallax);
            float Distance = Position.x * AmountOfParallax;

            Vector3 NewPosition = new Vector3(_startingPos + Distance, transform.position.y, transform.position.z);

            transform.position = NewPosition;

            // For future reference, rememeber not to do startpos +- (length/2) to avoid teleporting sprites
            if (Temp > _startingPos + _lengthOfSprite )
            {
                _startingPos += _lengthOfSprite;
            }
            else if (Temp < _startingPos - _lengthOfSprite )
            {
                _startingPos -= _lengthOfSprite;
            }
        }
    }
}