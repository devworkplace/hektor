var registrosObtidosParaTB_TIPO_LISTA = false;
var camposFormCadastro = {
    NOME: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'ID',
        name: 'ID'
    },
    {
        data: 'NOME',
        name: 'NOME'
    },
    {
        data: 'ID_PAI',
        name: 'ID_PAI'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'TB_TIPO_LISTA/Get';
metodoInsert = 'TB_TIPO_LISTA/Insert';

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

function inicializarRegistrosParaTB_TIPO_LISTA(chaveRegistro) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o registro</option>');
    $('#TB_TIPO_LISTA').append(auxOption);

    $.ajax({
        url: 'TB_TIPO_LISTA/GetParaChaveEstrangeira',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].chaveRegistro + '">' + dados[i].valorRegistro + '</option>');
                $('#TB_TIPO_LISTA').append(auxOption);
            }
            $('#TB_TIPO_LISTA').selectize();

            if (undefined == chaveRegistro) {
                $('#TB_TIPO_LISTA')[0].selectize.setValue('');
            } else {
                $('#TB_TIPO_LISTA')[0].selectize.setValue(chaveRegistro);
            }
            
            registrosObtidosParaTB_TIPO_LISTA = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#TB_TIPO_LISTA').selectize();
            mostrarMsgErro('Falha ao tentar obter os registros, favor tente novamente');
            fecharPopup();
        }
    });
}

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(false);

    $('#NOME').val('');

    if (registrosObtidosParaTB_TIPO_LISTA) {
        $('#TB_TIPO_LISTA')[0].selectize.setValue('');
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaTB_TIPO_LISTA,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarRegistrosParaTB_TIPO_LISTA
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(NOME, ID_PAI, ID) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        ID: ID
    });

    ajustarBotoes(false);

    $('#NOME').val(NOME);

    if (registrosObtidosParaTB_TIPO_LISTA) {
        $('#TB_TIPO_LISTA')[0].selectize.setValue(ID_PAI);
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaTB_TIPO_LISTA,
            idRegistroEscolhido: ID_PAI,
            funcaoInicializacao: inicializarRegistrosParaTB_TIPO_LISTA
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

metodoDetalhes = true;
function exibirDetalhes(NOME, ID_PAI, ID) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);

    $('#NOMELeitura').html(NOME);
    $('#TB_TIPO_LISTALeitura').html(ID_PAI);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'TB_TIPO_LISTA/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'ID\': \'' + objetoTabela.ID + '\' }';
}

metodoUpdate = 'TB_TIPO_LISTA/Update';
function colunasTabelaAlteracao(objetoTabela) {
    var auxID_PAI = isNullOrEmpty(objetoTabela.ID_PAI) ? '' : objetoTabela.ID_PAI;
    return '\'' + objetoTabela.NOME + '\', \'' + auxID_PAI + '\', \'' + objetoTabela.ID + '\'';
}

function montarDadosCadastro() {
    return {
        NOME: $('#NOME').val(),
        ID_PAI: $('#TB_TIPO_LISTA').val()
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
            auxData.ID = auxId.ID;
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