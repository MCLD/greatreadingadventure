using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace STG.CMS.Portal.Core
{
    [Serializable]
    public abstract class DataEntity
    {
        protected DataEntity()
        {
            InitFlags();
            _db = GenericSingleton<DAC>.GetInstance();
        }

        protected DataEntity(DAC db)
		{
            InitFlags();
			_db = db;
		}

        protected DataEntity(string connection)
        {
            InitFlags();
            _db = GenericSingleton<DAC>.GetInstance();
            _db.ConnectionString = connection;
        }

        private void InitFlags()
        {
            _isDirty = false;
            _isLoading = false;
            _isNew = true;
            _isDeleted = false;
            ChangedFields = new List<string>();
        }

        public virtual void EnrollInTransaction(DAC db)
        {
            //if (_db != db && db.Transaction != null)
            //    _db = db;
        }


        #region  Instance fields
        // Instance fields
        [CLSCompliantAttribute(false)]
        protected static DAC _db = GenericSingleton<DAC>.GetInstance();
        #endregion	

        #region  Instance Properties
        /// <summary>
        /// Gets the database object that this table belongs to.
        ///	</summary>
        public DAC Database
        {
            get { return _db; }
            set { _db = value; }

        }
        #endregion	

        private bool _isDeserializing;

        private bool _isLoading;
        protected bool IsLoading()
        {
            return _isLoading;
        }
        protected void StartLoading()
        {
            _isLoading = true;
        }
        protected void StopLoading()
        {
            _isLoading = false;
        }

        private bool _isDirty;
        public bool IsDirty()
        {
            return _isDirty;
        }
        protected void IsClean()
        {
            _isDirty = false;
            ChangedFields.Clear();
            IsOld();
        }

        private bool _isNew;
        public bool IsNew()
        {
            return _isNew;
        }
        protected void IsOld()
        {
            _isNew = false;
        }

        private bool _isDeleted;
        public bool IsDeleted()
        {
            return _isDeleted;
        }
        public void HasBeenDeleted()
        {
            ChangedFields.Clear();
            _isDeleted = true;
        }


        protected void SetValue<T>(ref T destination, ref T value, string name) where T : class
        {
            bool notifyChange = !_isDeserializing && !IsLoading();

            if (destination != null)
            {
                notifyChange = destination.Equals(value) ? false : notifyChange;
            }

            if (notifyChange)
            {
                NotifyPropertyChanged(name);
            }
            destination = value;
        }

        [OnDeserializing]
        void OnDeserializingMethod(StreamingContext context)
        {
            _isDeserializing = true;
        }

        [OnDeserialized]
        void OnDeserializedMethod(StreamingContext context)
        {
            _isDeserializing = false;
        }

        public List<String> ChangedFields { get; set; }

        private EditMode _editMode;
        public EditMode EditMode
        {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                if (_editMode == EditMode.Create || _editMode == EditMode.Delete || _editMode == EditMode.Edit)
                {
                    _isDirty = true;
                }
            }
        }


        public void NotifyPropertyChanged(string propertyName)
        {
            if (EditMode == EditMode.View)
                EditMode = EditMode.Edit;
            _isDirty = true;
            if (ChangedFields == null)
            {
                ChangedFields = new List<string>();
            }
            if (!ChangedFields.Contains(propertyName))
            {
                ChangedFields.Add(propertyName);
            }

        }

    }
}
