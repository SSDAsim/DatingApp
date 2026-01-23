import { Component, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css',
})
export class ImageUpload {
  // protected imgSrc? = signal<string | ArrayBuffer | null>(null);
  protected imgSrc = signal<string | ArrayBuffer | null | undefined>(null);
  protected isDragging = false; // property to check if a file is being dragged or not
  private fileToUpload: File | null = null;
  // we are using photo in the member-photo component (parent) so create an output
  uploadFile = output<File>();
  loading = input<boolean>(false);

  onDragOver(event: DragEvent){
    event.preventDefault();
    event.stopPropagation(); // does not allow any browser to handle image upload related properites
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent){
    event.preventDefault();
    event.stopPropagation(); // does not allow any browser to handle image upload related properites
    this.isDragging = false;
  }

  onDrop(event: DragEvent){
    event.preventDefault();
    event.stopPropagation(); // does not allow any browser to handle image upload related properites
    this.isDragging = false;

    // check if user has dropped any image and accept only first one as only one image at a time is allowed.
    if(event.dataTransfer?.files.length){
      const file = event.dataTransfer.files[0];
      this.previewImage(file);
      this.fileToUpload = file;
    }
  }

  onCancel() {
    this.fileToUpload = null;
    this.imgSrc.set(null);
  }

  onUploadFile() {
    if (this.fileToUpload)
    {
      this.uploadFile.emit(this.fileToUpload);
    }
  }

  // method to help with preview image 
  private previewImage(file: File){
    const reader = new FileReader(); // to read the contents of a file
    reader.onload = (e) => this.imgSrc.set(e.target?.result);
    reader.readAsDataURL(file); // this will give us the preview of the image
  }
}
