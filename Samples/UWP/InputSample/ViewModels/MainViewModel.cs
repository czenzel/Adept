// Copyright (c) Microsoft. All rights reserved.
//
using GalaSoft.MvvmLight;
using Adept.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InputSample.ViewModels
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    };

    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            left = 500;
            top = 500;
        }

        public void SetupInput()
        {
            // Get the move method
            var moveMethod = this.GetType().GetMethod(nameof(Move));

            // Define actions
            var moveLeftAction = new MethodAction(moveMethod, this)
            {
                Values =
                {
                    new ParameterValue()
                    {
                        Name = "direction",
                        Value = MoveDirection.Left
                    },
                    new ParameterValue()
                    {
                        Name = "amount",
                        Value = 20d
                    }
                }
            };

            var jumpLeftAction = new MethodAction(moveMethod, this)
            {
                Values =
                {
                    new ParameterValue()
                    {
                        Name = "direction",
                        Value = MoveDirection.Left
                    },
                    new ParameterValue()
                    {
                        Name = "amount",
                        Value = 100d
                    }
                }
            };

            var moveRightAction = new MethodAction(moveMethod, this)
            {
                Values =
                {
                    new ParameterValue()
                    {
                        Name = "direction",
                        Value = MoveDirection.Right
                    },
                    new ParameterValue()
                    {
                        Name = "amount",
                        Value = 20d
                    }
                }
            };

            var jumpRightAction = new MethodAction(moveMethod, this)
            {
                Values =
                {
                    new ParameterValue()
                    {
                        Name = "direction",
                        Value = MoveDirection.Right
                    },
                    new ParameterValue()
                    {
                        Name = "amount",
                        Value = 100d
                    }
                }
            };

            // Create key bindings
            var moveLeftKey = new KeyBinding()
            {
                Key = VirtualKey.Left,
                WhenRepeated = true,
                Action = moveLeftAction
            };

            var jumpLeftKey = new KeyBinding()
            {
                Key = VirtualKey.Left,
                Modifier = VirtualKey.Shift,
                WhenRepeated = true,
                Action = jumpLeftAction
            };

            var moveRightKey = new KeyBinding()
            {
                Key = VirtualKey.Right,
                WhenRepeated = true,
                Action = moveRightAction
            };

            var jumpRightKey = new KeyBinding()
            {
                Key = VirtualKey.Right,
                WhenRepeated = true,
                Modifier = VirtualKey.Shift,
                Action = jumpRightAction
            };

            // Create key bindings
            var moveLeftJoy = new KeyBinding()
            {
                Key = VirtualKey.GamepadDPadLeft,
                WhenRepeated = true,
                Action = moveLeftAction
            };

            var jumpLeftJoy = new KeyBinding()
            {
                Key = VirtualKey.GamepadDPadLeft,
                Modifier = VirtualKey.GamepadLeftShoulder,
                WhenRepeated = true,
                Action = jumpLeftAction
            };

            var moveRightJoy = new KeyBinding()
            {
                Key = VirtualKey.GamepadDPadRight,
                WhenRepeated = true,
                Action = moveRightAction
            };

            var jumpRightJoy = new KeyBinding()
            {
                Key = VirtualKey.GamepadDPadRight,
                WhenRepeated = true,
                Modifier = VirtualKey.GamepadLeftShoulder,
                Action = jumpRightAction
            };


            // Create keyboard mapper
            var keyMapper = new KeyboardMapper()
            {
                Bindings =
                {
                    moveLeftKey,
                    moveRightKey,
                    jumpLeftKey,
                    jumpRightKey,
                    moveLeftJoy,
                    moveRightJoy,
                    jumpLeftJoy,
                    jumpRightJoy
                }
            };

            // Start listening to events
            keyMapper.SubscribeEvents();
        }

        public void Move(MoveDirection direction, double amount)
        {
            switch (direction)
            {
                case MoveDirection.Down:
                    Top = top + amount;
                    break;
                case MoveDirection.Left:
                    Left = left - amount;
                    break;
                case MoveDirection.Right:
                    Left = left + amount;
                    break;
                case MoveDirection.Up:
                    Top = top - amount;
                    break;
            }
        }

        private double left;
        /// <summary>
        /// Gets or sets the left of the <see cref="MainViewModel"/>.
        /// </summary>
        /// <value>
        /// The left of the <c>MainViewModel</c>.
        /// </value>
        public double Left
        {
            get { return left; }
            set { Set(ref left, value); }
        }

        private double top;
        /// <summary>
        /// Gets or sets the top of the <see cref="MainViewModel"/>.
        /// </summary>
        /// <value>
        /// The top of the <c>MainViewModel</c>.
        /// </value>
        public double Top
        {
            get { return top; }
            set { Set(ref top, value); }
        }

    }
}
