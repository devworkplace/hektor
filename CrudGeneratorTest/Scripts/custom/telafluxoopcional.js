var registrosObtidosParaFluxo = false;
var camposFormCadastro = {
};
var colunasTabelaPrincipal = [
    {
        data: 'Id',
        name: 'Id'
    },
    {
        data: 'OrigemFluxo',
        name: 'OrigemFluxo'
    },
    {
        data: 'DestinoFluxo',
        name: 'DestinoFluxo'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'TelaFluxoOpcional/Get';
metodoInsert = 'TelaFluxoOpcional/Insert';

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

function inicializarRegistrosParaFluxo(chaveRegistro) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o registro</option>');
    $('#Fluxo').append(auxOption);

    $.ajax({
        url: 'Fluxo/GetParaChaveEstrangeira',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].chaveRegistro + '">' + dados[i].valorRegistro + '</option>');
                $('#Fluxo').append(auxOption);
            }
            $('#Fluxo').selectize();

            if (undefined == chaveRegistro) {
                $('#Fluxo')[0].selectize.setValue('');
            } else {
                $('#Fluxo')[0].selectize.setValue(chaveRegistro);
            }
            
            registrosObtidosParaFluxo = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#Fluxo').selectize();
            mostrarMsgErro('Falha ao tentar obter os registros, favor tente novamente');
            fecharPopup();
        }
    });
}

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(false);

    if (registrosObtidosParaFluxo) {
        $('#Fluxo')[0].selectize.setValue('');
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaFluxo,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarRegistrosParaFluxo
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(OrigemFluxo, DestinoFluxo, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        Id: Id
    });

    ajustarBotoes(false);

    if (registrosObtidosParaFluxo) {
        $('#Fluxo')[0].selectize.setValue(OrigemFluxo + ';' + DestinoFluxo);
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaFluxo,
            idRegistroEscolhido: OrigemFluxo + ';' + DestinoFluxo,
            funcaoInicializacao: inicializarRegistrosParaFluxo
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

metodoDetalhes = true;
function exibirDetalhes(OrigemFluxo, DestinoFluxo, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);

    $('#FluxoLeitura').html(OrigemFluxo + (isNullOrEmpty(DestinoFluxo) ? '' : ' - ' + DestinoFluxo));

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'TelaFluxoOpcional/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'Id\': \'' + objetoTabela.Id + '\' }';
}

metodoUpdate = 'TelaFluxoOpcional/Update';
function colunasTabelaAlteracao(objetoTabela) {
    var auxOrigemFluxo = isNullOrEmpty(objetoTabela.OrigemFluxo) ? '' : objetoTabela.OrigemFluxo;
    var auxDestinoFluxo = isNullOrEmpty(objetoTabela.DestinoFluxo) ? '' : objetoTabela.DestinoFluxo;
    return '\'' + auxOrigemFluxo + '\', \'' + auxDestinoFluxo + '\', \'' + objetoTabela.Id + '\'';
}

function montarDadosCadastro() {
    return {
        OrigemFluxo: $('#Fluxo').val().toString().split(';')[0],
        DestinoFluxo: $('#Fluxo').val().toString().split(';')[1]
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