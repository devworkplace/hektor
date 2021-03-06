var camposFormCadastro = {
    Nome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'Id',
        name: 'Id'
    },
    {
        data: 'Nome',
        name: 'Nome'
    },
    {
        data: 'Sobrenome',
        name: 'Sobrenome'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Pessoa/Get';
metodoInsert = 'Pessoa/Insert';

function ajustarBotoes(somenteLeitura) {
    if (somenteLeitura) {
        $('.campoCadastro').css('display', 'none');
        $('.campoLeitura').css('display', '');

        $('#btnSalvar').css('display', 'none');
        $('#btnCancelar').removeClass('btn-danger');
        $('#btnCancelar').addClass('btn-primary');
        $('#btnCancelar > label').html('Voltar');
    } else {
        $('.campoCadastro').css('display', '');
        $('.campoLeitura').css('display', 'none');

        $('#btnSalvar').css('display', '');
        $('#btnCancelar').removeClass('btn-primary');
        $('#btnCancelar').addClass('btn-danger');
        $('#btnCancelar > label').html('Cancelar');
    }
}

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(false);

    $('#Nome').val('');
    $('#Sobrenome').val('');

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

function carregarFormCadastro(Nome, Sobrenome, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        Id: Id
    });

    ajustarBotoes(false);

    $('#Nome').val(Nome);
    $('#Sobrenome').val(Sobrenome);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDetalhes = true;
function exibirDetalhes(Nome, Sobrenome, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);

    $('#NomeLeitura').html(Nome);
    $('#SobrenomeLeitura').html(Sobrenome);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'Pessoa/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'Id\': \'' + objetoTabela.Id + '\' }';
}

metodoUpdate = 'Pessoa/Update';
function colunasTabelaAlteracao(objetoTabela) {
    var auxSobrenome = isNullOrEmpty(objetoTabela.Sobrenome) ? '' : objetoTabela.Sobrenome;
    return '\'' + objetoTabela.Nome + '\', \'' + auxSobrenome + '\', \'' + objetoTabela.Id + '\'';
}

function montarDadosCadastro() {
    return {
        Nome: $('#Nome').val(),
        Sobrenome: $('#Sobrenome').val()
    };
}

$(document).ready(function () {
    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnSalvar').click(function () {
        var auxUrl;
        var auxData = montarDadosCadastro();

        var auxId = $(this).data('idRegistro');
        if (undefined == auxId) {
            auxUrl = metodoInsert;
        } else {
            auxUrl = metodoUpdate;
            auxData.Id = auxId.Id;
        }

        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData);
    });

    $('#btnSalvarContinuar').click(function () {
        var auxUrl = metodoInsert;
        var auxData = montarDadosCadastro();
        salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, true);
    });

    $('#btnCancelar').click(function () {
        mostrarDvTabelaPrincipal($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), undefined);
    });
});