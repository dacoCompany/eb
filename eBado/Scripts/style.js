// When the user press Esc button, close it
$(document).keyup(function (ev) {
    if (ev.keyCode == 27)
        $("#e-login-form").trigger("click");
});

// Get the modal
var modal = document.getElementById('e-login-form');
// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
