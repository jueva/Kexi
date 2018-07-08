using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media.Animation;
using Kexi.ViewModel;

namespace Kexi.View
{
    //https://www.codeproject.com/Articles/57175/WPF-How-To-Animate-Visibility-Property
    /// <summary>
    /// Supplies attached properties that provides visibility of animations
    /// </summary>
    public class VisibilityAnimation
    {
        public enum AnimationType
        {
            /// <summary>
            /// No animation
            /// </summary>
            None,

            /// <summary>
            /// Fade in / Fade out
            /// </summary>
            Fade
        }

        public enum SlideType
        {
            None,
            Left,
            Top,
            Right,
            Bottom
        }

        /// <summary>
        /// Animation duration
        /// </summary>
        private const int AnimationDuration = 200;

        /// <summary>
        /// List of hooked objects
        /// </summary>
        private static readonly Dictionary<FrameworkElement, bool> _hookedElements = 
            new Dictionary<FrameworkElement, bool>();

        /// <summary>
        /// Get AnimationType attached property
        /// </summary>
        /// <param name="obj">Dependency object</param>
        /// <returns>AnimationType value</returns>
        public static AnimationType GetAnimationType(DependencyObject obj)
        {
            return (AnimationType)obj.GetValue(AnimationTypeProperty);
        }

        /// <summary>
        /// Set AnimationType attached property
        /// </summary>
        /// <param name="obj">Dependency object</param>
        /// <param name="value">New value for AnimationType</param>
        public static void SetAnimationType(DependencyObject obj, AnimationType value)
        {
            obj.SetValue(AnimationTypeProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AnimationType. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AnimationTypeProperty = 
            DependencyProperty.RegisterAttached(
                "AnimationType",
                typeof(AnimationType),
                typeof(VisibilityAnimation),
                new FrameworkPropertyMetadata(AnimationType.None, 
                    new PropertyChangedCallback(OnAnimationTypePropertyChanged)));


        /// <summary>
        /// Get AnimationType attached property
        /// </summary>
        /// <param name="obj">Dependency object</param>
        /// <returns>AnimationType value</returns>
        public static SlideType GetSlideType(DependencyObject obj)
        {
            return (SlideType)obj.GetValue(SlideTypeProperty);
        }

        /// <summary>
        /// Set AnimationType attached property
        /// </summary>
        /// <param name="obj">Dependency object</param>
        /// <param name="value">New value for AnimationType</param>
        public static void SetSlideType(DependencyObject obj, SlideType value)
        {
            obj.SetValue(SlideTypeProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AnimationType. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SlideTypeProperty = 
            DependencyProperty.RegisterAttached(
                "SlideTypeProperty",
                typeof(SlideType),
                typeof(VisibilityAnimation),
                new FrameworkPropertyMetadata(SlideType.None, 
                    new PropertyChangedCallback(OnSlideTypePropertyChanged)));

        /// <summary>
        /// AnimationType property changed
        /// </summary>
        /// <param name="dependencyObject">Dependency object</param>
        /// <param name="e">e</param>
        private static void OnAnimationTypePropertyChanged(
            DependencyObject dependencyObject, 
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = dependencyObject as FrameworkElement;

            if (frameworkElement == null)
            {
                return;
            }

            // If AnimationType is set to True on this framework element, 
            if (GetAnimationType(frameworkElement) != AnimationType.None)
            {
                // Add this framework element to hooked list
                HookVisibilityChanges(frameworkElement);
            }
            else
            {
                // Otherwise, remove it from the hooked list
                UnHookVisibilityChanges(frameworkElement);
            }
        }

        /// <summary>
        /// SlideType property changed
        /// </summary>
        /// <param name="dependencyObject">Dependency object</param>
        /// <param name="e">e</param>
        private static void OnSlideTypePropertyChanged(
            DependencyObject                   dependencyObject, 
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement frameworkElement = dependencyObject as FrameworkElement;

            if (frameworkElement == null)
            {
                return;
            }

            // If AnimationType is set to True on this framework element, 
            if (GetSlideType(frameworkElement) != SlideType.None)
            {
                // Add this framework element to hooked list
                HookVisibilityChanges(frameworkElement);
            }
            else
            {
                // Otherwise, remove it from the hooked list
                UnHookVisibilityChanges(frameworkElement);
            }
        }

        /// <summary>
        /// Add framework element to list of hooked objects
        /// </summary>
        /// <param name="frameworkElement">Framework element</param>
        private static void HookVisibilityChanges(FrameworkElement frameworkElement)
        {
            if (!_hookedElements.ContainsKey(frameworkElement))
                _hookedElements.Add(frameworkElement, false);
        }

        /// <summary>
        /// Remove framework element from list of hooked objects
        /// </summary>
        /// <param name="frameworkElement">Framework element</param>
        private static void UnHookVisibilityChanges(FrameworkElement frameworkElement)
        {
            if (_hookedElements.ContainsKey(frameworkElement))
            {
                _hookedElements.Remove(frameworkElement);
            }
        }

        /// <summary>
        /// VisibilityAnimation static ctor
        /// </summary>
        static VisibilityAnimation()
        {
            // Here we "register" on Visibility property "before change" event
            UIElement.VisibilityProperty.AddOwner(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    Visibility.Visible, 
                    VisibilityChanged, 
                    CoerceVisibility));
        }

        /// <summary>
        /// Visibility changed
        /// </summary>
        /// <param name="dependencyObject">Dependency object</param>
        /// <param name="e">e</param>
        private static void VisibilityChanged(
            DependencyObject dependencyObject, 
            DependencyPropertyChangedEventArgs e)
        {
            // Ignore
        }

        /// <summary>
        /// Coerce visibility
        /// </summary>
        /// <param name="dependencyObject">Dependency object</param>
        /// <param name="baseValue">Base value</param>
        /// <returns>Coerced value</returns>
        private static object CoerceVisibility(
            DependencyObject dependencyObject, 
            object baseValue)
        {
            // Make sure object is a framework element
            FrameworkElement frameworkElement = dependencyObject as FrameworkElement;
            if (frameworkElement == null )
            {
                return baseValue;
            }

            // Cast to type safe value
            Visibility visibility = (Visibility)baseValue;

            // If Visibility value hasn't change, do nothing.
            // This can happen if the Visibility property is set using data binding 
            // and the binding source has changed but the new visibility value Hidden
            // hasn't changed.
            if (visibility == frameworkElement.Visibility)
            {
                return baseValue;
            }

            // If element is not hooked by our attached property, stop here
            if (!IsHookedElement(frameworkElement))
            {
                return baseValue;
            }

            // Update animation flag
            // If animation already started, don't restart it (otherwise, infinite loop)
            if (UpdateAnimationStartedFlag(frameworkElement))
            {
                return baseValue;
            }



            // If we get here, it means we have to start fade in or fade out animation. 
            // In any case return value of this method will be Visibility.Visible, 
            // to allow the animation.
            var doubleAnimation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(AnimationDuration))
            };

            var thicknessAnimation = GetThicknessAnimation(visibility, frameworkElement);

            // When animation completes, set the visibility value to the requested 
            // value (baseValue)
            doubleAnimation.Completed += (sender, eventArgs) =>
            {
                if (visibility == Visibility.Visible)
                {
                    // In case we change into Visibility.Visible, the correct value 
                    // is already set, so just update the animation started flag
                    UpdateAnimationStartedFlag(frameworkElement);
                }
                else
                {
                    // This will trigger value coercion again 
                    // but UpdateAnimationStartedFlag() function will return true 
                    // this time, thus animation will not be triggered. 
                    if (BindingOperations.IsDataBound(frameworkElement, 
                        UIElement.VisibilityProperty))
                    {
                        // Set visibility using bounded value
                        Binding bindingValue = 
                            BindingOperations.GetBinding(frameworkElement, 
                                UIElement.VisibilityProperty);
                        BindingOperations.SetBinding(frameworkElement, 
                            UIElement.VisibilityProperty, bindingValue);
                    }
                    else
                    {
                        // No binding, just assign the value
                        frameworkElement.Visibility = visibility;
                    }
                }
            };



            if (visibility == Visibility.Collapsed || visibility == Visibility.Hidden)
            {
                var workspace = KexContainer.Resolve<Workspace>();
                if (workspace.Options.PopupAnimation == PopupAnimation.None)
                {
                    return baseValue;
                }
                // Fade out by animating opacity
                doubleAnimation.From = frameworkElement.Opacity;
                doubleAnimation.To = 0.0;
            }
            else
            {
                // Fade in by animating opacity
                doubleAnimation.From = frameworkElement.Opacity;
                doubleAnimation.To = 1.0;
            }

                // Start animation
            if (doubleAnimation != null)
                frameworkElement.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
            if (thicknessAnimation != null)
                frameworkElement.BeginAnimation(FrameworkElement.MarginProperty, thicknessAnimation);

            // Make sure the element remains visible during the animation
            // The original requested value will be set in the completed event of 
            // the animation
            return Visibility.Visible;
        }

        private static ThicknessAnimation GetThicknessAnimation(Visibility visibility, FrameworkElement frameworkElement)
        {
            var slideType = GetSlideType(frameworkElement);
            if (slideType == SlideType.None)
                return null;

            var animation = new ThicknessAnimation
            {
                Duration          = new Duration(TimeSpan.FromMilliseconds(AnimationDuration)),
                DecelerationRatio = 0.7
            };
            var width = frameworkElement.ActualWidth;
            var height = 30;//frameworkElement.ActualHeight;
            if (visibility == Visibility.Collapsed || visibility == Visibility.Hidden)
            {
                //Slide Out
                switch (slideType)
                {
                        case SlideType.Bottom:
                            animation.From = frameworkElement.Margin;
                            animation.To   = new Thickness(0, 0, 0, -height);
                            break;
                        case SlideType.Left:
                            animation.From = frameworkElement.Margin;
                            animation.To   = new Thickness(-width, 0, 0, 0);
                            break;
                        case SlideType.Right:
                            animation.From = frameworkElement.Margin;
                            animation.To   = new Thickness(0, 0, -width, 0);
                            break;
                        case SlideType.Top:
                            animation.From = frameworkElement.Margin;
                            animation.To   = new Thickness(0, -height, 0, 0);
                            break;
                }
            }
            else
            {
                //Slide In
                switch (slideType)
                {
                        case SlideType.Bottom:
                            animation.From = new Thickness(0,0,0,-height);
                            animation.To   = new Thickness(0);
                            break;
                        case SlideType.Left:
                            animation.From = new Thickness(-width,0,0,0);
                            animation.To   = new Thickness(0);
                            break;
                        case SlideType.Right:
                            animation.From = new Thickness(0,0,-width,0);
                            animation.To   = new Thickness(0);
                            break;
                        case SlideType.Top:
                            animation.From = new Thickness(0,-height,0,0);
                            animation.To   = new Thickness(0);
                            break;
                }
            };
            return animation;
        }

        /// <summary>
        /// Check if framework element is hooked with AnimationType property
        /// </summary>
        /// <param name="frameworkElement">Framework element to check</param>
        /// <returns>Is the framework element hooked?</returns>
        private static bool IsHookedElement(FrameworkElement frameworkElement)
        {
            return _hookedElements.ContainsKey(frameworkElement);
        }

        /// <summary>
        /// Update animation started flag or a given framework element
        /// </summary>
        /// <param name="frameworkElement">Given framework element</param>
        /// <returns>Old value of animation started flag</returns>
        private static bool UpdateAnimationStartedFlag(FrameworkElement frameworkElement)
        {
            bool animationStarted = (bool)_hookedElements[frameworkElement];
            _hookedElements[frameworkElement] = !animationStarted;

            return animationStarted;
        }
    }
}