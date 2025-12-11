using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class CameraRotator : MonoBehaviour
    {
        public float RotateDuration;

        private float _currentRotation;
        private bool _isRotating;
        
        public void OnRotateLeft()
        {
            if (_isRotating) return;
            _currentRotation -= 90f;
            if (_currentRotation < -180f)
                _currentRotation = 90f;
            DOTween.Sequence()
                .Append(transform.DOLocalRotate(new(0, _currentRotation, 0), RotateDuration))
                .AppendCallback(() => { _isRotating = false;})
                .Play();
            _isRotating = true;
        }

        public void OnRotateRight()
        {
            if (_isRotating) return;
            _currentRotation += 90f;
            if (_currentRotation > 180f)
                _currentRotation = -90f;
            DOTween.Sequence()
                .Append(transform.DORotate(new(0, _currentRotation, 0), RotateDuration))
                .AppendCallback(() => { _isRotating = false;})
                .Play();
            _isRotating = true;
        }
    }
}
