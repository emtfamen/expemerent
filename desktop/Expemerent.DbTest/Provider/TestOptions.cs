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

        /// <summary>
        /// See <see cref="CreateDatabase"/> property
        /// </summary>
        private bool _createDatabase;

        /// <summary>
        /// See <see cref="SelectionTest"/> property
        /// </summary>
        private bool _selectionTest = true;

        /// <summary>
        /// See <see cref="ResultSetTest"/> property
        /// </summary>
        private bool _resultSetTest = true;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets "Create Database" flag
        /// </summary>
        public bool CreateDatabase
        {
            get { return _createDatabase; }
            set
            {
                _createDatabase = value;
                OnPropertyChanged("CreateDatabase");
            }
        }

        /// <summary>
        /// Gets or sets "selection test" flag
        /// </summary>
        public bool SelectionTest
        {
            get { return _selectionTest; }
            set
            {
                if (_selectionTest != value)
                {
                    _selectionTest = value;
                    OnPropertyChanged("SelectionTest");
                }
            }
        }

        /// <summary>
        /// Gets or sets "resultset test" flag
        /// </summary>
        public bool ResultSetTest
        {
            get { return _resultSetTest; }
            set
            {
                if (_resultSetTest != value)
                {
                    _resultSetTest = value;
                    OnPropertyChanged("ResultSetTest");
                }
            }
        }

        /// <summary>
        /// Gets or sets path to the database file
        /// </summary>
        public string DatabasePath
        {
            get { return _databasePath; }
            set
            {
                _databasePath = value;
                OnPropertyChanged("DatabasePath");
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
                OnPropertyChanged("RecordsCount");
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
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion        
    }
}
