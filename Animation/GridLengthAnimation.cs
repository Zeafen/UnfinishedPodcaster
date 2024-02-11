using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace VlogManager_Client.Animation
{
    public class GridLengthAnimation : AnimationTimeline
    {

        public GridLengthAnimation() { }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation), new PropertyMetadata(new GridLength(0, GridUnitType.Star)));

        public GridLength From
        {
            get => (GridLength)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));
        public GridLength To
        {
            get => (GridLength)GetValue(ToProperty);
            set
            {
                SetValue(ToProperty, value);
            }
        }
        public override Type TargetPropertyType
        {
            get => typeof(GridLength);
        }


        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromVal = this.From.Value;
            double toVal = this.To.Value;

            if (fromVal != toVal)
            {
                if (fromVal > toVal)
                    return new GridLength(1 - animationClock.CurrentProgress.Value * (fromVal - toVal) + toVal, this.To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
                else return new GridLength(animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal, this.From.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
            else return (GridLength)defaultDestinationValue;
        }
    }
    public class ExpanderDoubleAnimation : DoubleAnimationBase
    {

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(double?), typeof(ExpanderDoubleAnimation));

        public double? From
        {
            get
            {
                return (double?)GetValue(FromProperty);
            }
            set
            {
                SetValue(FromProperty, value);
            }
        }


        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(double?), typeof(ExpanderDoubleAnimation));


        public double? To
        {
            get
            {
                return (double?)GetValue(ExpanderDoubleAnimation.ToProperty);
            }
            set
            {
                SetValue(ExpanderDoubleAnimation.ToProperty, value);
            }
        }

        public double? ReverseValue
        {
            get { return (double)GetValue(ReverseValueProperty); }
            set { SetValue(ReverseValueProperty, value); }
        }

        public static readonly DependencyProperty ReverseValueProperty =
        DependencyProperty.Register("ReverseValue", typeof(double?), typeof(ExpanderDoubleAnimation), new UIPropertyMetadata(0.0));

        protected override Freezable CreateInstanceCore()
        {
            return new ExpanderDoubleAnimation();
        }

        protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock)
        {
            double fromVal = this.From.Value;
            double toVal = this.To.Value;

            if (defaultOriginValue == toVal)
            {
                fromVal = toVal;
                toVal = this.ReverseValue.Value;
            }
            if (fromVal > toVal)
                return (1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal;
            else
                return (animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal);
        }
    }
}
