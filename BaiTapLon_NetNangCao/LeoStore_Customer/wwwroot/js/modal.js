const btnformlogin=document.querySelector('.showformlogin');
const btnformregister=document.querySelector('.showformregister');
function showformregister(){
    document.querySelector('.modal').classList.add('open');
    document.querySelector('.form_register').style.display="block";
    document.querySelector('.form_login').style.display="none";
}
function showformlogin(){
    document.querySelector('.modal').classList.add('open');
    document.querySelector('.form_register').style.display="none";
    document.querySelector('.form_login').style.display="block";
}
function close()
{
    document.querySelector('.modal').classList.remove('open');
}
btnformlogin.addEventListener('click',showformlogin);
btnformregister.addEventListener('click',showformregister);
document.querySelector('.modal').addEventListener('click',close);
document.querySelector('.modal__body').addEventListener('click',function(event){
    event.stopPropagation()
});
document.getElementsByClassName('auth-form__switch-btn')[0].addEventListener('click',showformlogin);
document.getElementsByClassName('auth-form__switch-btn')[1].addEventListener('click',showformregister);
