package org.ico.main;

import org.neo.smartcontract.framework.services.neo.TriggerType;
import org.neo.smartcontract.framework.services.system.ExecutionEngine;

import java.math.BigInteger;

import org.ico.token.NEP5Template;
import org.ico.token.TemplateToken;
import org.neo.smartcontract.framework.Helper;
import org.neo.smartcontract.framework.SmartContract;
import org.neo.smartcontract.framework.services.neo.Runtime;
import org.neo.smartcontract.framework.services.neo.Storage;
import org.neo.smartcontract.framework.services.neo.Transaction;
import org.neo.smartcontract.framework.services.neo.TransactionOutput;

public class ICOTemplate extends SmartContract {

	/**
	 * The EPOC start time for the ICO. The associated time unit is seconds
	 */
	private final static long ICO_START_TIME = 1521003600;

	/**
	 * The EPOC end time for the ICO. The associated time unit is seconds
	 */
	private final static long ICO_END_TIME = 1523682000;

	/**
	 * The Entry point for the ICO Smart Contract. The neoj neo-compiler looks
	 * for a method named: Main with an uppercase M.
	 */
	public static Object Main(String operation, Object[] args) {
		TriggerType trigger = Runtime.trigger();

		if (trigger == TriggerType.Verification) {
			boolean isOwner = Runtime.checkWitness(TemplateToken.getOwner());

			if (isOwner) {
				return true;
			}
			return false;

		} else if (trigger == TriggerType.Application) {

			if (operation.equals("deploy")) {
				return deploy();
			} else if (operation.equals("mintTokens")) {
				return mintTokens();
			} else if (operation.equals(NEP5Template.TOTAL_SUPPLY)) {
				return NEP5Template.totalSupply();
			} else if (operation.equals(NEP5Template.NAME)) {
				return NEP5Template.name();
			} else if (operation.equals(NEP5Template.SYMBOL)) {
				return NEP5Template.symbol();
			} else if (operation.equals(NEP5Template.DECIMALS)) {
				return NEP5Template.decimalsString();
			} else if (operation.equals(NEP5Template.BALANCE_OF)) {
				if (args.length < 1) {
					Runtime.log("No account argument-No action");
					return false;
				}

				byte[] account = Helper.asByteArray((String) args[0]);
				return NEP5Template.balanceOf(account);
			} else if (operation.equals(NEP5Template.TRANSFER)) {
				if (args.length != 3) {
					Runtime.log("Insufficient number of arguments-No action");
					return false;
				}

				byte[] from = Helper.asByteArray((String) args[0]);
				byte[] to = Helper.asByteArray((String) args[1]);
				BigInteger amount = BigInteger.valueOf(Long.valueOf((String) args[2]));

				return NEP5Template.transfer(from, to, amount);
			}
		}

		return false;
	}

	public static Object deploy() {
		if (getTotalSupply().length != 0) {
			Runtime.log("Insufficient token supply-No action");
			return false;
		}

		Storage.put(Storage.currentContext(), TemplateToken.getOwner(), TemplateToken.getPreIcoCap());
		Storage.put(Storage.currentContext(), NEP5Template.TOTAL_SUPPLY, TemplateToken.getPreIcoCap());

		return true;
	}

	public static Object mintTokens() {
		byte[] sender = getSender();

		if (sender.length == 0) {
			Runtime.log("Asset is not neo-No action");
			return false;
		}

		// The current exchange rate between ICO tokens and Neo during the token
		// swap period
		BigInteger swapRate = currentSwapRate();
		if (swapRate.equals(BigInteger.ZERO)) {
			Runtime.log("Crowd funding failure-No action");
			return false;
		}

		BigInteger contributionAmount = getContributionAmount();
		BigInteger tokenTransferAmount = currentSwapToken(sender, contributionAmount, swapRate);
		if (tokenTransferAmount.equals(BigInteger.ZERO)) {
			Runtime.log("Zero token transfer amount-No action");
			return false;
		}

		byte[] senderBalanceByteArray = Storage.get(Storage.currentContext(), sender);
		BigInteger senderBalance = new BigInteger(senderBalanceByteArray);
		BigInteger postTransferAmount = senderBalance.add(tokenTransferAmount);
		Storage.put(Storage.currentContext(), sender, postTransferAmount);

		BigInteger tokenAndSupply = tokenTransferAmount.add(NEP5Template.totalSupply());
		Storage.put(Storage.currentContext(), NEP5Template.TOTAL_SUPPLY, tokenAndSupply);

		return true;
	}

	private static BigInteger currentSwapRate() {
		long icoDuration = ICO_END_TIME - ICO_START_TIME;

		long now = Runtime.time();
		long time = now - ICO_START_TIME;

		if (time < 0) {
			return BigInteger.ZERO;
		} else if (time < icoDuration) {
			return TemplateToken.getBasicRate();
		}

		return BigInteger.ZERO;
	}

	private static BigInteger getContributionAmount() {
		Transaction tx = (Transaction) ExecutionEngine.scriptContainer();
		TransactionOutput[] outputs = tx.outputs();
		BigInteger contributionAmount = BigInteger.ZERO;

		for (TransactionOutput transaction : outputs) {
			if (transaction.scriptHash() == getReceiver() && transaction.assetId() == neoAssetId()) {
				BigInteger transactionAmount = BigInteger.valueOf(transaction.value());
				contributionAmount = contributionAmount.add(transactionAmount);
			}
		}

		return contributionAmount;
	}

	private static BigInteger currentSwapToken(byte[] sender, BigInteger contributedAmount, BigInteger swapRate) {
		BigInteger tokenAmount = contributedAmount.divide(TemplateToken.getDecimals()).multiply(swapRate);
		BigInteger tokenBalance = TemplateToken.getTotalAmount().subtract(NEP5Template.totalSupply());

		if (tokenBalance.compareTo(BigInteger.ZERO) <= 0) {
			return BigInteger.ZERO;
		} else if (tokenBalance.compareTo(tokenAmount) < 0) {
			tokenAmount = tokenBalance;
		}

		return tokenAmount;
	}

	/**
	 * @return smart contract script hash
	 */
	private static byte[] getReceiver() {
		return ExecutionEngine.executingScriptHash();
	}

	private static byte[] getSender() {
		Transaction tx = (Transaction) ExecutionEngine.scriptContainer();
		TransactionOutput[] reference = tx.references();

		for (TransactionOutput output : reference) {
			if (output.assetId() == neoAssetId()) {
				return output.scriptHash();
			}
		}
		return Helper.asByteArray("");
	}

	private static byte[] getTotalSupply() {
		return Storage.get(Storage.currentContext(), NEP5Template.TOTAL_SUPPLY);
	}

	private static byte[] neoAssetId() {
		byte[] assetId = new byte[] { (byte) 155, (byte) 124, (byte) 255, (byte) 218, (byte) 166, (byte) 116,
				(byte) 190, (byte) 174, (byte) 15, (byte) 147, (byte) 14, (byte) 190, (byte) 96, (byte) 133, (byte) 175,
				(byte) 144, (byte) 147, (byte) 229, (byte) 254, (byte) 86, (byte) 179, (byte) 74, (byte) 92, (byte) 34,
				(byte) 12, (byte) 205, (byte) 207, (byte) 110, (byte) 252, (byte) 51, (byte) 111, (byte) 197 };
		return assetId;
	}

}
