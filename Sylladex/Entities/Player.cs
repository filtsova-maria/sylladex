﻿using Microsoft.Xna.Framework;
using Sylladex.Graphics;
using Sylladex.Managers;

namespace Sylladex.Entities
{
    /// <summary>
    /// Represents the facing direction of entities. Right is the default.
    /// </summary>
    public enum HorizontalDirection
    {
        Right = 1,
        Left = -1,
        None = 0
    }

    public enum VerticalDirection
    {
        Down = 1,
        Up = -1,
        None = 0
    }
    /// <summary>
    /// Represents the player entity in the game.
    /// </summary>
    public class Player : AnimatedEntity
    {
        private readonly Animation _moveAnimation;
        private readonly float _speed = 150F;
        /// <summary>
        /// Initializes a new instance of the Player class.
        /// </summary>
        /// <param name="pos">The initial position of the player.</param>
        public Player(Vector2 pos)
        {
            Sprite = new Sprite(this, GameManager.TextureManager.GetObject("player"));
            Direction = HorizontalDirection.Right;
            DrawPosition = pos;
            _moveAnimation = GameManager.AnimationManager.GetObject("playerMove");
        }

        /// <summary>
        /// Moves the player in the specified direction.
        /// </summary>
        /// <param name="xDirection">The horizontal direction of movement.</param>
        /// <param name="yDirection">The vertical direction of movement.</param>
        public void Move(HorizontalDirection xDirection, VerticalDirection yDirection)
        {
            // Set up the moving animation
            IsAnimating = true;
            CurrentAnimation = _moveAnimation;
            // Play walking sound effect
            GameManager.SoundEffectManager.Play("footsteps");
            // Calculate the new position with respect to game window border collisions
            float newX = DrawPosition.X + _speed * (int)xDirection * GameManager.DeltaTime;
            float newY = DrawPosition.Y + _speed * (int)yDirection * GameManager.DeltaTime;
            if (xDirection != 0)
            {
                Direction = xDirection;
            }

            if (newX >= 0 && newX <= GameManager.Graphics.PreferredBackBufferWidth - Sprite.GetWidth())
            {
                DrawPosition = new Vector2(newX, DrawPosition.Y);
            }

            if (newY >= 0 && newY <= GameManager.Graphics.PreferredBackBufferHeight - Sprite.GetHeight())
            {
                DrawPosition = new Vector2(DrawPosition.X, newY);
            }
        }
    }
}
