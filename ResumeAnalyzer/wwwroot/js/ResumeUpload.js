const fileInput = document.querySelector(".default-file-input");
const fileNameText = document.querySelector(".jDFileName");

fileInput.addEventListener("change", function () {
    if (this.files && this.files.length > 0) {
        fileNameText.textContent = this.files[0].name;
    } else {
        fileNameText.textContent = "Choose file";
    }
});

const resumeInput = document.querySelector(".default-file-input.Resume");
const resumeFileNameText = document.querySelector(".ResumeFileName");

resumeInput.addEventListener("change", function () {
    if (this.files && this.files.length > 0) {
        resumeFileNameText.textContent = this.files[0].name;
    } else {
        resumeFileNameText.textContent = "Choose file";
    }
});