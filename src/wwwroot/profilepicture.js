let _ppCropper = null;

function profilePic_openPicker() {
    document.getElementById('pic-file-input')?.click();
}

function profilePic_initCropper(dataUrl) {
    const img = document.getElementById('pic-crop-img');
    if (!img) return;
    if (_ppCropper) { _ppCropper.destroy(); _ppCropper = null; }
    img.src = dataUrl;
    img.onload = function () {
        _ppCropper = new Cropper(img, {
            aspectRatio: 1,
            viewMode: 1,
            dragMode: 'move',
            autoCropArea: 0.8,
            responsive: true,
            background: false
        });
    };
}

function profilePic_getCropped() {
    if (!_ppCropper) return null;
    const canvas = _ppCropper.getCroppedCanvas({ width: 256, height: 256 });
    return canvas ? canvas.toDataURL('image/jpeg', 0.9) : null;
}

function profilePic_destroyCropper() {
    if (_ppCropper) { _ppCropper.destroy(); _ppCropper = null; }
}
