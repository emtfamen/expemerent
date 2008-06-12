using System;
using Expemerent.UI.Dom;
using Expemerent.UI.Native;
using System.Drawing;
using System.Diagnostics;

namespace Expemerent.UI.Behaviors
{
    /// <summary>
    /// Type of the draw event
    /// </summary>
    public enum DrawEventType
    { 
        Background = DRAW_PARAMS.DRAW_EVENTS.DRAW_BACKGROUND,
        Content = DRAW_PARAMS.DRAW_EVENTS.DRAW_CONTENT,
        Foreground = DRAW_PARAMS.DRAW_EVENTS.DRAW_FOREGROUND,
    }

    /// <summary>
    /// Draw event args
    /// </summary>
    public class DrawEventArgs : ElementEventArgs
    {
        /// <summary>
        /// Graphics instance to paint 
        /// </summary>
        private Graphics _graphics;

        /// <summary>
        /// Creates a new instance of the <see cref="DrawEventArgs"/> class
        /// </summary>
        internal DrawEventArgs(Element element)
            : base(element)
        {
        }

        /// <summary>
        /// Gets type of the Draw event
        /// </summary>
        public DrawEventType EventType { get; protected internal set; }

        /// <summary>
        /// Returns current graphics instance
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                if (_graphics == null) 
                    _graphics = CreateGraphics();                    

                return _graphics;
            }
        }

        /// <summary>
        /// Gets size of the draw area
        /// </summary>
        public Rectangle DrawArea { get; protected internal set; }

        /// <summary>
        /// Device context to paint 
        /// </summary>
        protected internal IntPtr Hdc { get; set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Graphics"/> instance has been created
        /// </summary>
        protected internal bool IsGraphicsCreated 
        {
            [DebuggerStepThrough]
            get { return _graphics != null; } 
        }

        /// <summary>
        /// Creates a <see cref="Graphics"/> instance
        /// </summary>
        protected virtual Graphics CreateGraphics()
        {
            Handled = true;
            return Graphics.FromHdc(Hdc);
        }

        /// <summary>
        /// Releases created graphics instance
        /// </summary>
        protected internal virtual void ReleaseGraphics()
        {
            _graphics.Dispose();
            _graphics = null;
        }
    }
}