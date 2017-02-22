/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Tencent is pleased to support the open source community by making behaviac available.
//
// Copyright (C) 2015-2017 THL A29 Limited, a Tencent company. All rights reserved.
//
// Licensed under the BSD 3-Clause License (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at http://opensource.org/licenses/BSD-3-Clause
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Behaviac.Design;

namespace System.Drawing.Extended
{
    public class ExtendedGraphics
    {
        private Graphics mGraphics;
        public Graphics Graphics
        {
            get
            {
                return this.mGraphics;
            }
            set
            {
                this.mGraphics = value;
            }
        }


        public ExtendedGraphics(Graphics graphics)
        {
            this.Graphics = graphics;
        }


        #region Fills a Rounded Rectangle with continuous numbers.
        public void FillRoundRectangle(System.Drawing.Brush brush,
                                       float x, float y, float width, float height, float ratio)
        {
            Debug.Check(ratio >= 0.0f && ratio <= 0.5f);

            RectangleF rectangle = new RectangleF(x, y, width, height);
            GraphicsPath path = this.GetRoundedRect(rectangle, ratio);
            this.Graphics.FillPath(brush, path);
        }
        #endregion


        #region Fills a Corner Rectangle with continuous numbers.
        public void FillCornerRectangle(System.Drawing.Brush brush,
                                        float x, float y, float width, float height, float ratio)
        {
            Debug.Check(ratio >= 0.0f && ratio <= 0.5f);

            RectangleF rectangle = new RectangleF(x, y, width, height);
            GraphicsPath path = this.GetCornerRect(rectangle, ratio);
            this.Graphics.FillPath(brush, path);
        }
        #endregion


        #region Draws a Rounded Rectangle border with continuous numbers.
        public void DrawRoundRectangle(System.Drawing.Pen pen,
                                       float x, float y, float width, float height, float ratio)
        {
            Debug.Check(ratio >= 0.0f && ratio <= 0.5f);

            RectangleF rectangle = new RectangleF(x, y, width, height);
            GraphicsPath path = this.GetRoundedRect(rectangle, ratio);
            this.Graphics.DrawPath(pen, path);
        }
        #endregion


        #region Draws a Corner Rectangle border with continuous numbers.
        public void DrawCornerRectangle(System.Drawing.Pen pen,
                                        float x, float y, float width, float height, float ratio)
        {
            Debug.Check(ratio >= 0.0f && ratio <= 0.5f);

            RectangleF rectangle = new RectangleF(x, y, width, height);
            GraphicsPath path = this.GetCornerRect(rectangle, ratio);
            this.Graphics.DrawPath(pen, path);
        }
        #endregion


        #region Get the desired Rounded Rectangle path.
        private GraphicsPath GetRoundedRect(RectangleF baseRect, float ratio)
        {
            // if corner radius is less than or equal to zero,
            // return the original rectangle
            if (ratio <= 0.0f)
            {
                GraphicsPath mPath = new GraphicsPath();
                mPath.AddRectangle(baseRect);
                mPath.CloseFigure();
                return mPath;
            }

            // if the corner radius is greater than or equal to
            // half the width, or height (whichever is shorter)
            // then return a capsule instead of a lozenge
            if (ratio >= 0.5f)
            {
                return GetCapsule(baseRect);
            }

            float radius = Math.Min(baseRect.Width, baseRect.Height) * ratio;

            // create the arc for the rectangle sides and declare
            // a graphics path object for the drawing

            float diameter = radius * 2.0F;
            SizeF sizeF = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(baseRect.Location, sizeF);
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            // top left arc
            path.AddArc(arc, 180, 90);

            // top right arc
            arc.X = baseRect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc
            arc.Y = baseRect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc
            arc.X = baseRect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
        #endregion

        #region Gets the desired Capsular path.
        private GraphicsPath GetCapsule(RectangleF baseRect)
        {
            float diameter;
            RectangleF arc;
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            try
            {
                if (baseRect.Width > baseRect.Height)
                {
                    // return horizontal capsule
                    diameter = baseRect.Height;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(baseRect.Location, sizeF);
                    path.AddArc(arc, 90, 180);
                    arc.X = baseRect.Right - diameter;
                    path.AddArc(arc, 270, 180);

                }
                else if (baseRect.Width < baseRect.Height)
                {
                    // return vertical capsule
                    diameter = baseRect.Width;
                    SizeF sizeF = new SizeF(diameter, diameter);
                    arc = new RectangleF(baseRect.Location, sizeF);
                    path.AddArc(arc, 180, 180);
                    arc.Y = baseRect.Bottom - diameter;
                    path.AddArc(arc, 0, 180);

                }
                else
                {
                    // return circle
                    path.AddEllipse(baseRect);
                }

            }
            catch
            {
                path.AddEllipse(baseRect);
            }

            finally
            {
                path.CloseFigure();
            }

            return path;
        }
        #endregion

        #region Get the desired Corner Rectangle path.
        private GraphicsPath GetCornerRect(RectangleF baseRect, float ratio)
        {
            GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            float radius = Math.Min(baseRect.Width, baseRect.Height) * ratio;
            PointF[] points = new PointF[8];

            points[0].X = baseRect.Left;
            points[0].Y = baseRect.Top + radius;
            points[1].X = points[0].X + radius;
            points[1].Y = baseRect.Top;
            points[2].X = points[0].X + baseRect.Width - radius;
            points[2].Y = points[1].Y;
            points[3].X = points[0].X + baseRect.Width;
            points[3].Y = points[0].Y;
            points[4].X = points[3].X;
            points[4].Y = baseRect.Bottom - radius;
            points[5].X = points[2].X;
            points[5].Y = baseRect.Bottom;
            points[6].X = points[1].X;
            points[6].Y = points[5].Y;
            points[7].X = points[0].X;
            points[7].Y = points[4].Y;

            path.AddLines(points);
            path.CloseFigure();

            return path;
        }
        #endregion
    }
}