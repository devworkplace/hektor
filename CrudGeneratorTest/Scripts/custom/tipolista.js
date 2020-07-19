var registrosObtidosParaTipoLista = false;
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
        data: 'IdPai',
        name: 'IdPai'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'TipoLista/Get';
metodoInsert = 'TipoLista/Insert';

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

function inicializarRegistrosParaTipoLista(chaveRegistro) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o registro</option>');
    $('#TipoLista').append(auxOption);

    $.ajax({
        url: 'TipoLista/GetParaChaveEstrangeira',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].chaveRegistro + '">' + dados[i].valorRegistro + '</option>');
                $('#TipoLista').append(auxOption);
            }
            $('#TipoLista').selectize();

            if (undefined == chaveRegistro) {
                $('#TipoLista')[0].selectize.setValue('');
            } else {
                $('#TipoLista')[0].selectize.setValue(chaveRegistro);
            }
            
            registrosObtidosParaTipoLista = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#TipoLista').selectize();
            mostrarMsgErro('Falha ao tentar obter os registros, favor tente novamente');
            fecharPopup();
        }
    });
}

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(false);

    $('#Nome').val('');

    if (registrosObtidosParaTipoLista) {
        $('#TipoLista')[0].selectize.setValue('');
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaTipoLista,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarRegistrosParaTipoLista
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(Nome, IdPai, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        Id: Id
    });

    ajustarBotoes(false);

    $('#Nome').val(Nome);

    if (registrosObtidosParaTipoLista) {
        $('#TipoLista')[0].selectize.setValue(IdPai);
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaTipoLista,
            idRegistroEscolhido: IdPai,
            funcaoInicializacao: inicializarRegistrosParaTipoLista
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

metodoDetalhes = true;
function exibirDetalhes(Nome, IdPai, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);

    $('#NomeLeitura').html(Nome);
    $('#TipoListaLeitura').html(IdPai);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'TipoLista/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'Id\': \'' + objetoTabela.Id + '\' }';
}

metodoUpdate = 'TipoLista/Update';
function colunasTabelaAlteracao(objetoTabela) {
    var auxIdPai = isNullOrEmpty(objetoTabela.IdPai) ? '' : objetoTabela.IdPai;
    return '\'' + objetoTabela.Nome + '\', \'' + auxIdPai + '\', \'' + objetoTabela.Id + '\'';
}

function montarDadosCadastro() {
    return {
        Nome: $('#Nome').val(),
        IdPai: $('#TipoLista').val()
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