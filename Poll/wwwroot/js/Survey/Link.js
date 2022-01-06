let i = 1; 
let emailsContainer = document.getElementById("email-container");

function addChoice() {
    let element = document.createElement('div'); 
    element.classList.add('form-group');
    
    element.innerHTML = 
        '<input type="email" class="form-control" id="Emails_'+ i +'_" name="Emails['+ i +']"/>';
   

    emailsContainer.appendChild(element);
    i++;
}
