public class PersonagemScript
{
    private string[] scriptFalas;
    private string personagemCerne;

    public PersonagemScript()
    {
        ScriptFalas = new string[10];
        PersonagemCerne = "Sua miss�o � ser o mais parecido com um humano conversando num jogo de simula��o casual, para que o jogador pense que est� online. Suas respostas tem que ter, no m�ximo, " +
            "50 caracteres, encurtando mensagens longas para caber nesse crit�rio. Responda apenas com uma mensagem por vez";
    }

    public string[] ScriptFalas { get => scriptFalas; set => scriptFalas = value; }
    public string PersonagemCerne { get => personagemCerne; set => personagemCerne = value; }
}