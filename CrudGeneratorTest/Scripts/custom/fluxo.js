var camposFormCadastro = {
    Origem: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    Destino: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'Origem',
        name: 'Origem'
    },
    {
        data: 'Destino',
        name: 'Destino'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Fluxo/Get';
metodoInsert = 'Fluxo/Insert';

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

    $('#Origem').val('');
    $('#Destino').val('');

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

function carregarFormCadastro(Origem, Destino) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        Origem: Origem,
        Destino: Destino
    });

    ajustarBotoes(false);

    $('#Origem').val(Origem);
    $('#Destino').val(Destino);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDetalhes = true;
function exibirDetalhes(Origem, Destino) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);

    $('#OrigemLeitura').html(Origem);
    $('#DestinoLeitura').html(Destino);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'Fluxo/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'Origem\': \'' + objetoTabela.Origem + '\',\'Destino\': \'' + objetoTabela.Destino + '\' }';
}

metodoUpdate = 'Fluxo/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.Origem + '\', \'' + objetoTabela.Destino + '\'';
}

function montarDadosCadastro() {
    return {
        Origem: $('#Origem').val(),
        Destino: $('#Destino').val()
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
            auxData.Origem = auxId.Origem;
            auxData.Destino = auxId.Destino;
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