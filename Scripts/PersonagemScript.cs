public class PersonagemScript
{
    private string[] scriptFalas;
    private string personagemCerne;

    public PersonagemScript()
    {
        ScriptFalas = new string[10];
        PersonagemCerne = "Sua missão é ser o mais parecido com um humano conversando num jogo de simulação casual, para que o jogador pense que está online. Suas respostas tem que ter, no máximo, " +
            "50 caracteres, encurtando mensagens longas para caber nesse critério. Responda apenas com uma mensagem por vez";
    }

    public string[] ScriptFalas { get => scriptFalas; set => scriptFalas = value; }
    public string PersonagemCerne { get => personagemCerne; set => personagemCerne = value; }
}