package org.ico.token;

import java.math.BigInteger;

import org.neo.smartcontract.framework.services.neo.Runtime;
import org.neo.smartcontract.framework.services.neo.Storage;

/**
 * NEP-5 outlines a token standard for the NEO blockchain that will provide
 * systems with a generalized interaction mechanism for tokenized Smart
 * Contracts.
 */
public class NEP5Template {
	public final static String TOTAL_SUPPLY = "totalSupply";
	public final static String NAME = "name";
	public final static String SYMBOL = "symbol";
	public final static String DECIMALS = "decimals";
	public final static String BALANCE_OF = "balanceOf";
	public final static String TRANSFER = "transfer";

	/**
	 * @return the total token supply deployed in the system
	 */
	public static BigInteger totalSupply() {
		byte[] totalSupply = Storage.get(Storage.currentContext(), "totalSupply");
		return new BigInteger(totalSupply);
	}

	/**
	 * @return the token name
	 */
	public static String name() {
		return TemplateToken.getName();
	}

	/**
	 * @return the token symbol
	 */
	public static String symbol() {
		return TemplateToken.getSymbol();
	}

	/**
	 * @return the number of decimals used by the token
	 */
	public static byte decimals() {
		return TemplateToken.decimals();
	}

	/**
	 * @return the number of decimals used by the token as a String
	 */
	public static String decimalsString() {
		return String.valueOf(TemplateToken.decimals());
	}

	/**
	 * @return the token balance of the account
	 */
	public static BigInteger balanceOf(byte[] account) {
		return new BigInteger(Storage.get(Storage.currentContext(), account));
	}

	/**
	 * Will transfer an amount of tokens from the from account to the to account
	 */
	public static Boolean transfer(byte[] from, byte[] to, BigInteger amount) {
		if (amount.compareTo(BigInteger.ZERO) <= 0) {
			Runtime.log("No transfer amount-No action");
			return false;
		}
		
		if (to.length != 20) return false;

		if (!Runtime.checkWitness(from)) {
			Runtime.log("Not transfering from self-No action");
			return false;
		}

		byte[] senderSupplyByteArray = Storage.get(Storage.currentContext(), from);
		BigInteger senderSupply = new BigInteger(senderSupplyByteArray);

		if (senderSupply.compareTo(amount) < 0) {
			Runtime.log("Insufficient funds for sender-No action");
			return false;
		}
		
		if (from == to) {
			Runtime.log("Transfering to self-No action");
			return true;
		}

		if (senderSupply == amount) {
			Storage.delete(Storage.currentContext(), from);
		} else {
			BigInteger remainingSupply = senderSupply.subtract(amount);
			Storage.put(Storage.currentContext(), from, remainingSupply);
		}

		byte[] receiverSupplyByteArray = Storage.get(Storage.currentContext(), to);
		BigInteger receiverSupply = new BigInteger(receiverSupplyByteArray);

		BigInteger postTransferAmount = receiverSupply.add(amount);
		Storage.put(Storage.currentContext(), to, postTransferAmount);

		return true;
	}

}
