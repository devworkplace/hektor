var registrosObtidosParaDepartamento = false;
var camposFormCadastro = {
    Nome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    Sobrenome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    Departamento: {
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
        data: 'Departamento',
        name: 'Departamento'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Funcionario/Get';
metodoInsert = 'Funcionario/Insert';

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

function inicializarRegistrosParaDepartamento(chaveRegistro) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o registro</option>');
    $('#Departamento').append(auxOption);

    $.ajax({
        url: 'Departamento/GetParaChaveEstrangeira',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].chaveRegistro + '">' + dados[i].valorRegistro + '</option>');
                $('#Departamento').append(auxOption);
            }
            $('#Departamento').selectize();

            if (undefined == chaveRegistro) {
                $('#Departamento')[0].selectize.setValue('');
            } else {
                $('#Departamento')[0].selectize.setValue(chaveRegistro);
            }
            $('#formCadastro').data('bootstrapValidator').updateStatus('Departamento', 'NOT_VALIDATED');
            
            registrosObtidosParaDepartamento = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#Departamento').selectize();
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
    $('#Sobrenome').val('');

    if (registrosObtidosParaDepartamento) {
        $('#Departamento')[0].selectize.setValue('');
        $('#formCadastro').data('bootstrapValidator').updateStatus('Departamento', 'NOT_VALIDATED');
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaDepartamento,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarRegistrosParaDepartamento
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(Nome, Sobrenome, Departamento, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        Id: Id
    });

    ajustarBotoes(false);

    $('#Nome').val(Nome);
    $('#Sobrenome').val(Sobrenome);

    if (registrosObtidosParaDepartamento) {
        $('#Departamento')[0].selectize.setValue(Departamento);
        $('#formCadastro').data('bootstrapValidator').updateStatus('Departamento', 'NOT_VALIDATED');
    }    

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaDepartamento,
            idRegistroEscolhido: Departamento,
            funcaoInicializacao: inicializarRegistrosParaDepartamento
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

metodoDetalhes = true;
function exibirDetalhes(Nome, Sobrenome, Departamento, Id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);

    $('#NomeLeitura').html(Nome);
    $('#SobrenomeLeitura').html(Sobrenome);
    $('#DepartamentoLeitura').html(Departamento);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'Funcionario/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'Id\': \'' + objetoTabela.Id + '\' }';
}

metodoUpdate = 'Funcionario/Update';
function colunasTabelaAlteracao(objetoTabela) {
    return '\'' + objetoTabela.Nome + '\', \'' + objetoTabela.Sobrenome + '\', \'' + objetoTabela.Departamento + '\', \'' + objetoTabela.Id + '\'';
}

function montarDadosCadastro() {
    return {
        Nome: $('#Nome').val(),
        Sobrenome: $('#Sobrenome').val(),
        Departamento: $('#Departamento').val()
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