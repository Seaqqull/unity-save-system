using Vector2 = UnityEngine.Vector2;
using SaveSystem.Examples.Data;
using UnityEngine.EventSystems;
using SaveSystem.Data;
using UnityEngine;


namespace SaveSystem.Examples
{
    public class DraggableEntity : LocationItem, 
        IBeginDragHandler, IDragHandler, IEndDragHandler,
        IPointerClickHandler 
    {
        [SerializeField] private RectTransform _bounds;
        [SerializeField] private GameObject _dummy;
        [SerializeField] private GameObject _selection;
        [SerializeField] private GameObject _delete;
        [Space] 
        [SerializeField] private Vector2 _size;

        private Transform _dummyTransform;
        private Vector2 _halfBoundsSize;
        private Vector2 _scaledRectSize;
        private Transform _transform;
        private Vector2 _halfSize;
        private Vector2 _rectSize;


        protected override void Awake()
        {
            _rectSize = _bounds.rect.size;

            _dummyTransform = _dummy.transform;
            _transform = transform;
            
            base.Awake();
        }

        private void Start()
        {
            _scaledRectSize = _bounds.rect.size;
            var scaledSize = Vector2.Scale(
                _size, 
                new Vector2((_rectSize.x / _scaledRectSize.x), (_rectSize.y / _scaledRectSize.y)));

            _halfBoundsSize = Vector2.Scale(_scaledRectSize, new Vector2(0.5f, 0.5f));
            _halfSize = Vector2.Scale(scaledSize, new Vector2(0.5f, 0.5f));
        }
        
        private void OnDestroy()
        {
            Location.RemoveGlobally(this);
        }


        private Vector2 GetClampedPosition(Vector2 position)
        {
            if (position.x < _halfSize.x)
                position.x = _halfSize.x;
            else if (position.x > (_rectSize.x - _halfSize.x))
                position.x = (_rectSize.x - _halfSize.x);
            if (position.y < _halfSize.y)
                position.y = _halfSize.y;
            else if (position.y > (_rectSize.y - _halfSize.y))
                position.y = (_rectSize.y - _halfSize.y);

            return new Vector2(
                (position.x / _rectSize.x) * _scaledRectSize.x, 
                (position.y / _rectSize.y) * _scaledRectSize.y);
        }

        public void Destroy()
        {
            Exists = false;
            Destroy(gameObject);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            _dummyTransform.localPosition = (GetClampedPosition(eventData.position) - _halfBoundsSize);
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _transform.localPosition = (GetClampedPosition(eventData.position) - _halfBoundsSize);

            _selection.SetActive(false);
            _dummy.SetActive(false);
            
            Location.AddGlobally(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnDrag(eventData);
            
            _selection.SetActive(true);
            _dummy.SetActive(true);
        }

        
        public void OnPointerClick(PointerEventData eventData)
        {
            _delete.SetActive(!_delete.activeSelf);
            _selection.SetActive(false);
        }

        #region Savable
        public override SaveSnap MakeSnap()
        {
            return new DraggableItemSnap(Id, _transform.localPosition, Exists);
        }

        public override void FromSnap(SaveSnap data)
        {
            var itemData = data as DraggableItemSnap;
            if ((itemData == null) || !itemData.Id.Equals(Id))
                return;

            _transform.localPosition = itemData.Position;
            Exists = itemData.Exists;
            
            Location.AddGlobally(this);

            if (!Exists)
                Destroy(gameObject);
        }
        #endregion
    }
}