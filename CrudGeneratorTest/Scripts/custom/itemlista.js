var registrosObtidosParaItemLista = false;
var registrosObtidosParaTipoLista = false;
var camposFormCadastro = {
    Nome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    TipoLista: {
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
        data: 'TipoLista',
        name: 'TipoLista'
    },
    {
        data: 'IdPai',
        name: 'IdPai'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'ItemLista/Get';
metodoInsert = 'ItemLista/Insert';

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
            $('#formCadastro').data('bootstrapValidator').updateStatus('TipoLista', 'NOT_VALIDATED');
            
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

function inicializarRegistrosParaItemLista(chaveRegistro) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o registro</option>');
    $('#ItemLista').append(auxOption);

    $.ajax({
        url: 'ItemLista/GetParaChaveEstrangeira',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].chaveRegistro + '">' + dados[i].valorRegistro + '</option>');
                $('#ItemLista').append(auxOption);
            }
            $('#ItemLista').selectize();

            if (undefined == chaveRegistro) {
                $('#ItemLista')[0].selectize.setValue('');
            } else {
                $('#ItemLista')[0].selectize.setValue(chaveRegistro);
            }
            
            registrosObtidosParaItemLista = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#ItemLista').selectize();
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
        $('#formCadastro').data('bootstrapValidator').updateStatus('TipoLista', 'NOT_VALIDATED');
    }    

    if (registrosObtidosParaItemLista) {
        $('#ItemLista')[0].selectize.setValue('');
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaTipoLista,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarRegistrosParaTipoLista
        },
        {
            registrosObtidos: registrosObtidosParaItemLista,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarRegistrosParaItemLista
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(Nome, TipoLista, IdPai, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        Id: Id
    });

    ajustarBotoes(false);

    $('#Nome').val(Nome);

    if (registrosObtidosParaTipoLista) {
        $('#TipoLista')[0].selectize.setValue(TipoLista);
        $('#formCadastro').data('bootstrapValidator').updateStatus('TipoLista', 'NOT_VALIDATED');
    }    

    if (registrosObtidosParaItemLista) {
        $('#ItemLista')[0].selectize.setValue(IdPai);
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaTipoLista,
            idRegistroEscolhido: TipoLista,
            funcaoInicializacao: inicializarRegistrosParaTipoLista
        },
        {
            registrosObtidos: registrosObtidosParaItemLista,
            idRegistroEscolhido: IdPai,
            funcaoInicializacao: inicializarRegistrosParaItemLista
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

metodoDetalhes = true;
function exibirDetalhes(Nome, TipoLista, IdPai, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);

    $('#NomeLeitura').html(Nome);
    $('#TipoListaLeitura').html(TipoLista);
    $('#ItemListaLeitura').html(IdPai);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'ItemLista/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'Id\': \'' + objetoTabela.Id + '\' }';
}

metodoUpdate = 'ItemLista/Update';
function colunasTabelaAlteracao(objetoTabela) {
    var auxIdPai = isNullOrEmpty(objetoTabela.IdPai) ? '' : objetoTabela.IdPai;
    return '\'' + objetoTabela.Nome + '\', \'' + objetoTabela.TipoLista + '\', \'' + auxIdPai + '\', \'' + objetoTabela.Id + '\'';
}

function montarDadosCadastro() {
    return {
        Nome: $('#Nome').val(),
        TipoLista: $('#TipoLista').val(),
        IdPai: $('#ItemLista').val()
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