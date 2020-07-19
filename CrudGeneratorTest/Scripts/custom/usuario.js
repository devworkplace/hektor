var registrosObtidosParaUsuario = false;
var camposFormCadastro = {
    login: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    senha: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    cpf: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            },
            callback: {
                message: 'CPF inv&aacute;lido',
                callback: function (value) {
                    return cpfValido(value);
                }
            }
        }
    },
    nome: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    email: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    },
    tipo: {
        validators: {
            notEmpty: {
                message: 'Campo obrigat&oacute;rio'
            }
        }
    }
};
var colunasTabelaPrincipal = [
    {
        data: 'cpf',
        name: 'cpf'
    },
    {
        data: 'nome',
        name: 'nome'
    },
    {
        data: 'email',
        name: 'email'
    },
    {
        data: 'tipoGrid',
        name: 'tipo'
    },
    {
        data: 'usuarioCriador.nome',
        name: 'nomeUsuarioCriador'
    },
    {
        data: 'dataAdmissaoGrid',
        name: 'dataAdmissao'
    },
    {
        render: renderColunaOpcoes
    }
];
metodoGet = 'Usuario/Get';
metodoInsert = 'Usuario/Insert';

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
    limparListaLinhasSelect();
}

function inicializarRegistrosParaUsuario(chaveRegistro) {
    mostrarPopup();

    var auxOption = $('<option value="">Informe o registro</option>');
    $('#Usuario').append(auxOption);

    $.ajax({
        url: 'Usuario/GetParaChaveEstrangeira',
        type: 'POST',
        success: function (result) {
            var dados = result.data;
            for (var i = 0; i < dados.length; i++) {
                auxOption = $('<option value="' + dados[i].chaveRegistro + '">' + dados[i].valorRegistro + '</option>');
                $('#Usuario').append(auxOption);
            }
            $('#Usuario').selectize();

            if (undefined == chaveRegistro) {
                $('#Usuario')[0].selectize.setValue('');
            } else {
                $('#Usuario')[0].selectize.setValue(chaveRegistro);
            }

            registrosObtidosParaUsuario = true;

            fecharPopup();
        },
        error: function (request, status, error) {
            $('#Usuario').selectize();
            mostrarMsgErro('Falha ao tentar obter os registros, favor tente novamente');
            fecharPopup();
        }
    });
}

function limparFormCadastro() {
    $('#btnSalvarContinuar').css('display', '');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(false);

    $('#login').val('');
    $('#senha').val('');
    $('#cpf').val('');
    $('#nome').val('');
    $('#email').val('');
    $('#tipo')[0].selectize.setValue('');
    $('#formCadastro').data('bootstrapValidator').updateStatus('tipo', 'NOT_VALIDATED');
    $('#dataAdmissao').val('');

    if (registrosObtidosParaUsuario) {
        $('#Usuario')[0].selectize.setValue('');
    }

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaUsuario,
            idRegistroEscolhido: undefined,
            funcaoInicializacao: inicializarRegistrosParaUsuario
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

function carregarFormCadastro(login, senha, cpf, nome, email, tipo, idUsuarioCriador, nomeUsuarioCriador, dataAdmissaoGrid, id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').data('idRegistro', {
        id: id
    });

    ajustarBotoes(false);

    $('#login').val(login);
    $('#senha').val(senha);
    $('#cpf').val(cpf);
    $('#nome').val(nome);
    $('#email').val(email);
    $('#tipo')[0].selectize.setValue(tipo);
    $('#dataAdmissao').datepicker('setDate', dataAdmissaoGrid);

    if (registrosObtidosParaUsuario) {
        $('#Usuario')[0].selectize.setValue(idUsuarioCriador);
    }

    var listaSelects =
    [
        {
            registrosObtidos: registrosObtidosParaUsuario,
            idRegistroEscolhido: idUsuarioCriador,
            funcaoInicializacao: inicializarRegistrosParaUsuario
        }
    ];

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'), listaSelects);
}

metodoDetalhes = true;
function exibirDetalhes(login, senha, cpf, nome, email, tipo, idUsuarioCriador, nomeUsuarioCriador, dataAdmissao, id) {
    $('#btnSalvarContinuar').css('display', 'none');
    $('#btnSalvar').removeData('idRegistro');

    ajustarBotoes(true);

    $('#cpfLeitura').html(cpf);
    $('#nomeLeitura').html(nome);
    $('#emailLeitura').html(email);
    $('#tipoLeitura').html(tipo);
    $('#UsuarioLeitura').html(nomeUsuarioCriador);
    $('#dataAdmissaoLeitura').html(dataAdmissao);

    mostrarDvFormCadastro($('#dvFormCadastro'), $('#dvTabelaPrincipal'));
}

metodoDelete = 'Usuario/Delete';
function colunasTabelaRemocao(objetoTabela) {
    return '{ \'id\': \'' + objetoTabela.id + '\' }';
}

metodoUpdate = 'Usuario/Update';
function colunasTabelaAlteracao(objetoTabela) {
    var auxIdUsuarioCriador = isNullOrEmpty(objetoTabela.usuarioCriador) ? '' : objetoTabela.usuarioCriador.id;
    var auxNomeUsuarioCriador = isNullOrEmpty(objetoTabela.usuarioCriador) ? '' : objetoTabela.usuarioCriador.nome;
    var auxdataAdmissao = isNullOrEmpty(objetoTabela.dataAdmissaoGrid) ? '' : objetoTabela.dataAdmissaoGrid;
    return '\'' + objetoTabela.login + '\', \'' + objetoTabela.senha + '\', \'' + objetoTabela.cpf + '\', \'' + objetoTabela.nome + '\', \'' + objetoTabela.email + '\', \'' + objetoTabela.tipo + '\', \'' + auxIdUsuarioCriador + '\', \'' + auxNomeUsuarioCriador + '\', \'' + auxdataAdmissao + '\', \'' + objetoTabela.id + '\'';
}

metodoSelectLinha = true;
function obterIdParaCheckBox(objetoTabela) {
    return objetoTabela.id;
}

function obterIdParaCheckBoxDataTable(objetoTabela) {
    return '\'' + objetoTabela.id + '\'';
}

function montarDadosCadastro() {
    return {
        login: $('#login').val(),
        senha: $('#senha').val(),
        cpf: $('#cpf').val(),
        nome: $('#nome').val(),
        email: $('#email').val(),
        tipo: $('#tipo').val(),
        dataAdmissao: $('#dataAdmissao').val(),
        idUsuarioCriador: $('#Usuario').val()
    };
}

$(document).ready(function () {
    iniciarListaLinhasSelect();

    inicializarTabelaPrincipal($('#tabelaPrincipal'), colunasTabelaPrincipal);

    $('#cpf').mask('000.000.000-00', { reverse: true });

    $('#tipo').selectize();

    inicializarCampoData($('#dataAdmissao'), $('#formCadastro'));

    options.fields = camposFormCadastro;
    $('#formCadastro').bootstrapValidator(options);

    $('#btnNovo').click(function () {
        limparFormCadastro();
    });

    $('#btnExcluirItens').click(function () {
        var auxLinhasSelect = obterLinhasSelect();
        if (isNullOrEmpty(auxLinhasSelect)) {
            mostrarMsgErro('Nenhum registro foi selecionado');
        } else {
            var auxUrl = 'Usuario/ExcluirMultiplos';
            var auxData = {
                idsUsuarios: auxLinhasSelect.split(';')
            };
            salvarRegistro($('#formCadastro'), $('#dvFormCadastro'), $('#dvTabelaPrincipal'), auxUrl, auxData, undefined, true, false, true);
        }
    });

    $('#btnSalvar').click(function () {
        var auxUrl;
        var auxData = montarDadosCadastro();

        var auxId = $(this).data('idRegistro');
        if (undefined == auxId) {
            auxUrl = metodoInsert;
        } else {
            auxUrl = metodoUpdate;
            auxData.id = auxId.id;
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