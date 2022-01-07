let i = 2; 
let choiceContainer = document.getElementById("choice-container");

function addChoice() {
    let element = document.createElement('div'); 
    element.classList.add('form-group');
    
    element.innerHTML = 
        '<label class="control-label"> Choix '+ (i+1) +'</label>' +
    '<input type="text" class="form-control" id="Choices_' + i + '_" name="Choices[' + i +']" maxlength="50"/>' +
        '<span class="text-danger field-validation-valid" data-valmsg-for="Choices['+ i +']" data-valmsg-replace="true"></span>';
   

    choiceContainer.appendChild(element);
    i++;
}
