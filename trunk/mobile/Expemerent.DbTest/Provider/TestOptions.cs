using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Expemerent.DbTest.Provider
{
    /// <summary>
    /// Defines a possible test options
    /// </summary>
    public class TestOptions : INotifyPropertyChanged
    {
        #region Private data
        /// <summary>
        /// Cached instance
        /// </summary>
        private static readonly PropertyChangedEventArgs PropertyChangedEvent = new PropertyChangedEventArgs(String.Empty);

        /// <summary>
        /// See <see cref="CreateNewDatabase"/> property
        /// </summary>
        private int _recordsCount;

        /// <summary>
        /// See <see cref="DatabaseFile"/> property
        /// </summary>
        private string _databasePath;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets path to the database file
        /// </summary>
        public string DatabasePath
        {
            get { return _databasePath; }
            set
            {
                _databasePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets amount of records in test operation
        /// </summary>
        public int RecordsCount
        {
            get { return _recordsCount; }
            set
            {
                _recordsCount = value;
                OnPropertyChanged();
            }
        } 
        #endregion

        #region INotifyPropertyChanged implementation
        /// <summary>
        /// Occurs when property has been changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> event
        /// </summary>
        protected void OnPropertyChanged()
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, PropertyChangedEvent);
        } 
        #endregion        
    }
}
